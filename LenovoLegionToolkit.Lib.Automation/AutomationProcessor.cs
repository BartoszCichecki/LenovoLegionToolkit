using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.AutoListeners;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Automation;

public class AutomationProcessor(
    AutomationSettings settings,
    DisplayConfigurationListener displayConfigurationListener,
    NativeWindowsMessageListener nativeWindowsMessageListener,
    PowerStateListener powerStateListener,
    PowerModeListener powerModeListener,
    GodModeController godModeController,
    GameAutoListener gameAutoListener,
    ProcessAutoListener processAutoListener,
    TimeAutoListener timeAutoListener,
    UserInactivityAutoListener userInactivityAutoListener,
    WiFiAutoListener wifiAutoListener)
{
    private readonly AsyncLock _ioLock = new();
    private readonly AsyncLock _runLock = new();

    private List<AutomationPipeline> _pipelines = [];
    private CancellationTokenSource? _cts;

    public bool IsEnabled => settings.Store.IsEnabled;

    public event EventHandler<List<AutomationPipeline>>? PipelinesChanged;

    #region Initialization / pipeline reloading

    public async Task InitializeAsync()
    {
        using (await _ioLock.LockAsync().ConfigureAwait(false))
        {
            displayConfigurationListener.Changed += DisplayConfigurationListener_Changed;
            nativeWindowsMessageListener.Changed += NativeWindowsMessageListener_Changed;
            powerStateListener.Changed += PowerStateListener_Changed;
            powerModeListener.Changed += PowerModeListener_Changed;
            godModeController.PresetChanged += GodModeController_PresetChanged;

            _pipelines = [.. settings.Store.Pipelines];

            RaisePipelinesChanged();

            await UpdateListenersAsync().ConfigureAwait(false);
        }
    }

    public async Task SetEnabledAsync(bool enabled)
    {
        using (await _ioLock.LockAsync().ConfigureAwait(false))
        {
            settings.Store.IsEnabled = enabled;
            settings.SynchronizeStore();

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

            settings.Store.Pipelines = pipelines;
            settings.SynchronizeStore();

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

        Task.Run(() => ProcessEvent(new StartupAutomationEvent()));
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

            if (_cts is not null)
                await _cts.CancelAsync().ConfigureAwait(false);

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
                    if (pipeline.Trigger is null || !await pipeline.Trigger.IsMatchingEvent(automationEvent).ConfigureAwait(false))
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

    private async void DisplayConfigurationListener_Changed(object? sender, DisplayConfigurationListener.ChangedEventArgs args)
    {
        var e = new HDRAutomationEvent(args.HDR);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void NativeWindowsMessageListener_Changed(object? sender, NativeWindowsMessageListener.ChangedEventArgs args)
    {
        var e = new NativeWindowsMessageEvent(args.Message, args.Data);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void PowerStateListener_Changed(object? sender, PowerStateListener.ChangedEventArgs args)
    {
        var e = new PowerStateAutomationEvent(args.PowerStateEvent, args.PowerAdapterStateChanged);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void PowerModeListener_Changed(object? sender, PowerModeListener.ChangedEventArgs args)
    {
        var e = new PowerModeAutomationEvent(args.State);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void GodModeController_PresetChanged(object? sender, Guid presetId)
    {
        var e = new CustomModePresetAutomationEvent(presetId);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void GameAutoListener_Changed(object? sender, GameAutoListener.ChangedEventArgs args)
    {
        var e = new GameAutomationEvent(args.Running);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void ProcessAutoListener_Changed(object? sender, ProcessAutoListener.ChangedEventArgs args)
    {
        var e = new ProcessAutomationEvent(args.Type, args.ProcessInfo);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void TimeAutoListener_Changed(object? sender, TimeAutoListener.ChangedEventArgs args)
    {
        var e = new TimeAutomationEvent(args.Time, args.Day);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void UserInactivityAutoListener_Changed(object? sender, UserInactivityAutoListener.ChangedEventArgs args)
    {
        var e = new UserInactivityAutomationEvent(args.TimerResolution * args.TickCount);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    private async void WiFiAutoListener_Changed(object? sender, WiFiAutoListener.ChangedEventArgs args)
    {
        var e = new WiFiAutomationEvent(args.IsConnected, args.Ssid);
        await ProcessEvent(e).ConfigureAwait(false);
    }

    #endregion

    #region Event processing

    private async Task ProcessEvent(IAutomationEvent e)
    {
        var potentialMatch = _pipelines.SelectMany(p => p.AllTriggers)
            .Select(async t => await t.IsMatchingEvent(e).ConfigureAwait(false))
            .Select(t => t.Result)
            .Where(t => t)
            .Any();

        if (!potentialMatch)
            return;

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Processing event {e}... [type={e.GetType().Name}]");

        await RunAsync(e).ConfigureAwait(false);
    }

    #endregion

    #region Helper methods

    private async Task UpdateListenersAsync()
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Stopping listeners...");

        await gameAutoListener.UnsubscribeChangedAsync(GameAutoListener_Changed).ConfigureAwait(false);
        await processAutoListener.UnsubscribeChangedAsync(ProcessAutoListener_Changed).ConfigureAwait(false);
        await timeAutoListener.UnsubscribeChangedAsync(TimeAutoListener_Changed).ConfigureAwait(false);
        await userInactivityAutoListener.UnsubscribeChangedAsync(UserInactivityAutoListener_Changed).ConfigureAwait(false);
        await wifiAutoListener.UnsubscribeChangedAsync(WiFiAutoListener_Changed).ConfigureAwait(false);

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

        var triggers = _pipelines.SelectMany(p => p.AllTriggers).ToArray();

        if (triggers.OfType<IGameAutomationPipelineTrigger>().Any())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting game listener...");

            await gameAutoListener.SubscribeChangedAsync(GameAutoListener_Changed).ConfigureAwait(false);
        }

        if (triggers.OfType<IProcessesAutomationPipelineTrigger>().Any())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting process listener...");

            await processAutoListener.SubscribeChangedAsync(ProcessAutoListener_Changed).ConfigureAwait(false);
        }

        if (triggers.OfType<ITimeAutomationPipelineTrigger>().Any() || triggers.OfType<IPeriodicAutomationPipelineTrigger>().Any())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting time listener...");

            await timeAutoListener.SubscribeChangedAsync(TimeAutoListener_Changed).ConfigureAwait(false);
        }

        if (triggers.OfType<IUserInactivityPipelineTrigger>().Any())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting user inactivity listener...");

            await userInactivityAutoListener.SubscribeChangedAsync(UserInactivityAutoListener_Changed).ConfigureAwait(false);
        }

        if (triggers.OfType<IWiFiConnectedPipelineTrigger>().Any() || triggers.OfType<WiFiDisconnectedAutomationPipelineTrigger>().Any())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting WiFi listener...");

            await wifiAutoListener.SubscribeChangedAsync(WiFiAutoListener_Changed).ConfigureAwait(false);
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
