using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Automation
{
    public class AutomationProcessor
    {
        private readonly AutomationSettings _settings;
        private readonly PowerStateListener _powerStateListener;
        private readonly ProcessListener _processListener;

        private readonly AsyncLock _ioLock = new();
        private readonly AsyncLock _runLock = new();

        private List<AutomationPipeline> _pipelines = new();
        private CancellationTokenSource? _cts;

        public bool IsEnabled
        {
            get => _settings.Store.IsEnabled;
            set
            {
                using (_ioLock.Lock())
                {
                    _settings.Store.IsEnabled = value;
                    _settings.SynchronizeStore();
                }
            }
        }

        public event EventHandler<List<AutomationPipeline>>? PipelinesChanged;

        public AutomationProcessor(AutomationSettings settings, PowerStateListener powerStateListener, ProcessListener processListener)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _powerStateListener = powerStateListener ?? throw new ArgumentNullException(nameof(powerStateListener));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));

            _powerStateListener.Changed += PowerStateListener_Changed;
            _processListener.Changed += ProcessListener_Changed;
        }

        private async void ProcessListener_Changed(object? sender, ProcessEventInfo e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is IProcessesAutomationPipelineTrigger)
                .Where(t => t?.IsSatisfied(e) ?? false)
                .Any();

            if (!potentialMatch)
                return;

            await RunAsync(e);
        }

        private async void PowerStateListener_Changed(object? sender, EventArgs e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is IPowerAutomationPipelineTrigger)
                .Where(t => t?.IsSatisfied(e) ?? false)
                .Any();

            if (!potentialMatch)
                return;

            await RunAsync(e);
        }

        public async Task InitializeAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _pipelines = _settings.Store.Pipelines;
                RaisePipelinesChanged();
            }
        }

        public async Task ReloadPipelinesAsync(List<AutomationPipeline> pipelines)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipelines reload pending...");

            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipelines reloading...");

                _pipelines = pipelines.Select(p => p.DeepCopy()).ToList();

                _settings.Store.Pipelines = pipelines;
                _settings.SynchronizeStore();

                RaisePipelinesChanged();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipelines reloaded.");
            }
        }

        public async Task<List<AutomationPipeline>> GetPipelinesAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
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

            using (await _runLock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipeline run starting...");

                try
                {
                    await pipeline.DeepCopy().RunAsync().ConfigureAwait(false);

                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Pipeline run finished successfully.");
                }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Pipeline run failed.", ex);

                    throw;
                }
            }
        }

        private Task RunAsync() => RunAsync(null);

        private async Task RunAsync(object? context)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run pending...");

            using (await _runLock.LockAsync().ConfigureAwait(false))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run starting...");

                _cts?.Cancel();

                if (!IsEnabled)
                    return;

                List<AutomationPipeline> pipelines;
                using (await _ioLock.LockAsync().ConfigureAwait(false))
                    pipelines = _pipelines;

                _cts = new CancellationTokenSource();
                var ct = _cts.Token;

                foreach (var pipeline in pipelines)
                {
                    if (ct.IsCancellationRequested)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Run interrupted.");
                        break;
                    }

                    try
                    {
                        if (pipeline.Trigger is null || !pipeline.Trigger.IsSatisfied(context))
                        {
                            if (Log.Instance.IsTraceEnabled)
                                Log.Instance.Trace($"Pipeline triggers not satisfied. [name={pipeline.Name}, trigger={pipeline.Trigger}, steps.Count={pipeline.Steps.Count}]");
                            continue;
                        }

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Running pipeline... [name={pipeline.Name}, trigger={pipeline.Trigger}, steps.Count={pipeline.Steps.Count}]");

                        await pipeline.RunAsync(ct).ConfigureAwait(false);

                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Pipeline completed successfully. [name={pipeline.Name}, trigger={pipeline.Trigger}]");
                    }
                    catch (Exception ex)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Pipeline run failed. [name={pipeline.Name}, trigger={pipeline.Trigger}]", ex);
                    }

                    if (pipeline.IsExclusive)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Pipeline is exclusive. Breaking. [name={pipeline.Name}, trigger={pipeline.Trigger}, steps.Count={pipeline.Steps.Count}]");
                        break;
                    }
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Run finished successfully.");
            }
        }

        private void RaisePipelinesChanged()
        {
            PipelinesChanged?.Invoke(this, _pipelines.Select(p => p.DeepCopy()).ToList());
        }
    }
}
