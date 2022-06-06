using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

#pragma warning disable CA1822 // Mark members as static

namespace LenovoLegionToolkit.Lib.Automation
{
    public class AutomationProcessor
    {
        public class PipelinesChangedEventArgs : EventArgs
        {
            public List<AutomationPipeline> Pipelines { get; }

            public PipelinesChangedEventArgs(List<AutomationPipeline> pipelines)
            {
                Pipelines = pipelines;
            }
        }

        private readonly PowerStateListener _powerStateListener;
        private readonly AsyncLock _lock = new();

        private List<AutomationPipeline> _pipelines = new();
        private CancellationTokenSource? _cts;

        public bool IsEnabled
        {
            get => AutomationSettings.Instance.IsEnabled;
            set
            {
                AutomationSettings.Instance.IsEnabled = value;
                AutomationSettings.Instance.Synchronize();
            }
        }

        public event EventHandler<PipelinesChangedEventArgs>? PipelinesChanged;

        public AutomationProcessor(PowerStateListener powerStateListener)
        {
            _powerStateListener = powerStateListener;
            _powerStateListener.Changed += PowerStateListener_Changed;
        }

        private async void PowerStateListener_Changed(object? sender, EventArgs e)
        {
            await RunAsync();
        }

        public async Task InitializeAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                _pipelines = AutomationSettings.Instance.Pipeliness;

                PipelinesChanged?.Invoke(this, new(_pipelines.Select(p => p.DeepCopy()).ToList()));
            }
        }

        public async Task ReloadPipelinesAsync(List<AutomationPipeline> pipelines)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipelines reload pending...");

            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipelines reloading...");

                _pipelines = pipelines.Select(p => p.DeepCopy()).ToList();

                AutomationSettings.Instance.Pipeliness = pipelines;
                AutomationSettings.Instance.Synchronize();

                PipelinesChanged?.Invoke(this, new(_pipelines.Select(p => p.DeepCopy()).ToList()));

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipelines reloaded.");
            }
        }

        public async Task<List<AutomationPipeline>> GetPipelinesAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
                return _pipelines.Select(p => p.DeepCopy()).ToList();
        }

        public void RunOnStartup()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipeline run on startup pending...");

            _ = Task.Run(RunAsync);
        }

        public async Task RunNowAsync(AutomationPipeline pipeline)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipeline run now pending...");

            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipeline run starting...");

                try
                {
                    await pipeline.DeepCopy().RunAsync(force: true).ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Pipeline run finished successfully.");
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Pipeline run failed: {ex.Demystify()}");

                    throw;
                }
            }
        }

        private async Task RunAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run pending...");

            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run starting...");

                _cts?.Cancel();

                if (!IsEnabled)
                    return;

                _cts = new CancellationTokenSource();
                var ct = _cts.Token;

                foreach (var pipeline in _pipelines)
                {
                    if (ct.IsCancellationRequested)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Run interrupted.");
                        break;
                    }

                    try
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Running pipeline... [name={pipeline.Name}, triggers={string.Join(",", pipeline.Triggers)}, steps.Count={pipeline.Steps.Count}]");

                        await pipeline.RunAsync(token: ct).ConfigureAwait(false);

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Pipeline completed successfully. [name={pipeline.Name}, triggers={string.Join(",", pipeline.Triggers)}]");
                    }
                    catch (Exception ex)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Pipeline run failed: {ex.Demystify()} [name={pipeline.Name}, triggers={string.Join(",", pipeline.Triggers)}]");
                    }
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run finished successfully.");
            }
        }
    }
}
