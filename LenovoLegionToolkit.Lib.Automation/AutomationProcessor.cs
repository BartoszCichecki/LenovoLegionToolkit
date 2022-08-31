using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Listeners;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Automation
{
    public class AutomationProcessor
    {
        private readonly AutomationSettings _settings;
        private readonly PowerStateAutomationListener _powerStateListener;
        private readonly ProcessAutomationListener _processListener;
        private readonly TimeAutomationListener _timeListener;

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

                    UpdateListeners();
                }
            }
        }

        public event EventHandler<List<AutomationPipeline>>? PipelinesChanged;

        public AutomationProcessor(AutomationSettings settings,
            PowerStateAutomationListener powerStateListener,
            ProcessAutomationListener processListener,
            TimeAutomationListener timeListener)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _powerStateListener = powerStateListener ?? throw new ArgumentNullException(nameof(powerStateListener));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
            _timeListener = timeListener ?? throw new ArgumentNullException(nameof(timeListener));
        }

        private async void PowerStateListener_Changed(object? sender, EventArgs _)
        {
            var e = new PowerAutomationEvent();

            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is IPowerAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            await RunAsync(e).ConfigureAwait(false);
        }

        private async void ProcessListener_Changed(object? sender, ProcessEventInfo processEventInfo)
        {
            var e = new ProcessAutomationEvent { ProcessEventInfo = processEventInfo };

            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is IProcessesAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            await RunAsync(e).ConfigureAwait(false);
        }

        private async void TimeListener_Changed(object? sender, Time time)
        {
            var e = new TimeAutomationEvent { Time = time };

            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is TimeAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            await RunAsync(e).ConfigureAwait(false);
        }

        public async Task InitializeAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _powerStateListener.Changed += PowerStateListener_Changed;
                _processListener.Changed += ProcessListener_Changed;
                _timeListener.Changed += TimeListener_Changed;

                _pipelines = _settings.Store.Pipelines;

                RaisePipelinesChanged();
                UpdateListeners();
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

                UpdateListeners();

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

            _ = Task.Run(() => RunAsync(new StartupAutomationEvent()));
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

        private async Task RunAsync(IAutomationEvent automationEvent)
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
                        if (pipeline.Trigger is null || !await pipeline.Trigger.IsSatisfiedAsync(automationEvent).ConfigureAwait(false))
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

        private void UpdateListeners()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping listeners...");

            _timeListener.Stop();
            _processListener.Stop();
            _powerStateListener.Stop();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped listeners...");

            if (!IsEnabled)
            {

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not enabled. Will not start listeners.");
                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting listeners...");

            var triggers = _pipelines.Select(p => p.Trigger);

            if (triggers.OfType<TimeAutomationPipelineTrigger>().Any())
                _timeListener.Start();

            if (triggers.OfType<IProcessesAutomationPipelineTrigger>().Any())
                _processListener.Start();

            _powerStateListener.Start();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started listeners.");
        }

        private void RaisePipelinesChanged()
        {
            PipelinesChanged?.Invoke(this, _pipelines.Select(p => p.DeepCopy()).ToList());
        }
    }
}
