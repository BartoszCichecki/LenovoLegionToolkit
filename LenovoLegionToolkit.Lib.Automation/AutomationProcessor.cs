using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Listeners;
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
        private readonly PowerModeListener _powerModeListener;
        private readonly ProcessAutomationListener _processListener;
        private readonly TimeAutomationListener _timeListener;

        private readonly AsyncLock _ioLock = new();
        private readonly AsyncLock _runLock = new();

        private List<AutomationPipeline> _pipelines = new();
        private CancellationTokenSource? _cts;

        public bool IsEnabled => _settings.Store.IsEnabled;

        public event EventHandler<List<AutomationPipeline>>? PipelinesChanged;

        public AutomationProcessor(AutomationSettings settings,
            PowerStateListener powerStateListener,
            PowerModeListener powerModeListener,
            ProcessAutomationListener processListener,
            TimeAutomationListener timeListener)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _powerStateListener = powerStateListener ?? throw new ArgumentNullException(nameof(powerStateListener));
            _powerModeListener = powerModeListener ?? throw new ArgumentNullException(nameof(powerModeListener));
            _processListener = processListener ?? throw new ArgumentNullException(nameof(processListener));
            _timeListener = timeListener ?? throw new ArgumentNullException(nameof(timeListener));
        }

        #region Initialization / pipeline reloading

        public async Task InitializeAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _powerStateListener.Changed += PowerStateListener_Changed;
                _powerModeListener.Changed += PowerModeListenerOnChanged;
                _processListener.Changed += ProcessListener_Changed;
                _timeListener.Changed += TimeListener_Changed;

                _pipelines = _settings.Store.Pipelines;

                RaisePipelinesChanged();

                await UpdateListenersAsync().ConfigureAwait(false);
            }
        }

        public async Task SetEnabledAsync(bool enabled)
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
            {
                _settings.Store.IsEnabled = enabled;
                _settings.SynchronizeStore();

                await UpdateListenersAsync().ConfigureAwait(false);
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

                await UpdateListenersAsync().ConfigureAwait(false);

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Pipelines reloaded.");
            }
        }

        public async Task<List<AutomationPipeline>> GetPipelinesAsync()
        {
            using (await _ioLock.LockAsync().ConfigureAwait(false))
                return _pipelines.Select(p => p.DeepCopy()).ToList();
        }

        #endregion

        #region Run

        public void RunOnStartup()
        {
            if (!IsEnabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Not enabled. Pipeline run on startup ignored.");

                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipeline run on startup pending...");

            Task.Run(() => RunAsync(new StartupAutomationEvent()));
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

        public async Task RunNowAsync(Guid pipelineId)
        {
            using (await _runLock.LockAsync().ConfigureAwait(false))
            {
                var pipeline = _pipelines.Where(p => p.Trigger is null).FirstOrDefault(p => p.Id == pipelineId);
                if (pipeline is null)
                    return;

                await RunNowAsync(pipeline).ConfigureAwait(false);
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

        #endregion

        #region Listeners

        private async void PowerStateListener_Changed(object? sender, EventArgs _)
        {
            var e = new PowerStateAutomationEvent();
            await ProcessEvent(e).ConfigureAwait(false);
        }

        private async void PowerModeListenerOnChanged(object? sender, PowerModeState powerModeState)
        {
            var e = new PowerModeAutomationEvent { PowerModeState = powerModeState };
            await ProcessEvent(e).ConfigureAwait(false);
        }

        private async void ProcessListener_Changed(object? sender, ProcessEventInfo processEventInfo)
        {
            var e = new ProcessAutomationEvent { ProcessEventInfo = processEventInfo };
            await ProcessEvent(e).ConfigureAwait(false);
        }

        private async void TimeListener_Changed(object? sender, Time time)
        {
            var e = new TimeAutomationEvent { Time = time };
            await ProcessEvent(e).ConfigureAwait(false);
        }

        #endregion

        #region Event processing

        private async Task ProcessEvent(PowerStateAutomationEvent e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is IPowerStateAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Processing power state event.");

            await RunAsync(e).ConfigureAwait(false);
        }

        private async Task ProcessEvent(PowerModeAutomationEvent e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is IPowerModeAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Processing power mode event. [powerModeState={e.PowerModeState}]");

            await RunAsync(e).ConfigureAwait(false);
        }

        private async Task ProcessEvent(ProcessAutomationEvent e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is IProcessesAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Processing process event. [process.Name={e.ProcessEventInfo.Process.Name}, type={e.ProcessEventInfo.Type}]");

            await RunAsync(e).ConfigureAwait(false);
        }

        private async Task ProcessEvent(TimeAutomationEvent e)
        {
            var potentialMatch = _pipelines.Select(p => p.Trigger)
                .Where(t => t is not null)
                .Where(t => t is TimeAutomationPipelineTrigger)
                .Select(async t => await t!.IsSatisfiedAsync(e).ConfigureAwait(false))
                .Select(t => t.Result)
                .Where(t => t)
                .Any();

            if (!potentialMatch)
                return;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Processing time event. [time={e.Time.Hour}:{e.Time.Minute}]");

            await RunAsync(e).ConfigureAwait(false);
        }

        #endregion

        #region Helper methods

        private async Task UpdateListenersAsync()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopping listeners...");

            await _timeListener.StopAsync().ConfigureAwait(false);
            await _processListener.StopAsync().ConfigureAwait(false);

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

            var triggers = _pipelines.Select(p => p.Trigger).ToArray();

            if (triggers.OfType<IProcessesAutomationPipelineTrigger>().Any())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Starting process listener...");

                await _processListener.StartAsync().ConfigureAwait(false);
            }

            if (triggers.OfType<TimeAutomationPipelineTrigger>().Any())
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Starting time listener...");

                await _timeListener.StartAsync().ConfigureAwait(false);
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Started relevant listeners.");
        }

        private void RaisePipelinesChanged()
        {
            PipelinesChanged?.Invoke(this, _pipelines.Select(p => p.DeepCopy()).ToList());
        }

        #endregion

    }
}
