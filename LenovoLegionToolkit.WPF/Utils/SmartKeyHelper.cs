﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils;

internal class SmartKeyHelper
{
    private readonly TimeSpan _smartKeyDoublePressInterval = TimeSpan.FromMilliseconds(500);

    private DateTime _lastSmartKeyPress = DateTime.MinValue;
    private CancellationTokenSource? _smartKeyDoublePressCancellationTokenSource;

    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
    private readonly FnKeysDisabler _fnKeysDisabler = IoCContainer.Resolve<FnKeysDisabler>();
    private readonly SpecialKeyListener _specialKeyListener = IoCContainer.Resolve<SpecialKeyListener>();
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

    public Action? BringToForeground { get; set; }

    private static SmartKeyHelper? _instance;

    public static SmartKeyHelper Instance => _instance ??= new SmartKeyHelper();

    private SmartKeyHelper()
    {
        _specialKeyListener.Changed += SpecialKeyListener_Changed;
    }

    private async void SpecialKeyListener_Changed(object? sender, SpecialKeyListener.ChangedEventArgs e)
    {
        if (e.SpecialKey != SpecialKey.FnF9)
            return;

        if (await _fnKeysDisabler.GetStatusAsync() == SoftwareStatus.Enabled)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Ignoring Fn+F9 FnKeys are enabled.");

            return;
        }

        if (_smartKeyDoublePressCancellationTokenSource is not null)
            await _smartKeyDoublePressCancellationTokenSource.CancelAsync();
        _smartKeyDoublePressCancellationTokenSource = new CancellationTokenSource();

        var token = _smartKeyDoublePressCancellationTokenSource.Token;

        _ = Task.Run(async () =>
        {
            var now = DateTime.UtcNow;
            var diff = now - _lastSmartKeyPress;
            _lastSmartKeyPress = now;

            if (diff < _smartKeyDoublePressInterval)
            {
                await ProcessSpecialKey(true);
                return;
            }

            await Task.Delay(_smartKeyDoublePressInterval, token);
            await ProcessSpecialKey(false);
        }, token);
    }

    private async Task ProcessSpecialKey(bool isDoublePress)
    {
        var currentGuid = isDoublePress
            ? _settings.Store.SmartKeyDoublePressActionId
            : _settings.Store.SmartKeySinglePressActionId;
        var actionList = isDoublePress
            ? _settings.Store.SmartKeyDoublePressActionList
            : _settings.Store.SmartKeySinglePressActionList;

        if (!currentGuid.HasValue)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Bringing to foreground after {(isDoublePress ? "double" : "single")} Fn+F9 press.");
            BringToForeground?.Invoke();
            return;
        }

        if (currentGuid.Value == Guid.Empty)
            return;

        if (actionList.IsEmpty())
            actionList.Add(currentGuid.Value);

        var currentIndex = Math.Max(0, actionList.IndexOf(currentGuid.Value));
        var nextIndex = (currentIndex + 1) % actionList.Count;

        currentGuid = actionList[currentIndex];

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Running action {currentGuid} after {(isDoublePress ? "double" : "single")} Fn+F9 press.");

        try
        {
            var pipelines = await _automationProcessor.GetPipelinesAsync();
            var pipeline = pipelines.FirstOrDefault(p => p.Id == currentGuid);
            if (pipeline is not null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Running action {currentGuid} after {(isDoublePress ? "double" : "single")} Fn+F9 press.");

                await _automationProcessor.RunNowAsync(pipeline.Id);

                MessagingCenter.Publish(new NotificationMessage(isDoublePress ? NotificationType.SmartKeyDoublePress : NotificationType.SmartKeySinglePress, pipeline.Name ?? string.Empty));
            }
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Running action {currentGuid} after {(isDoublePress ? "double" : "single")} Fn+F9 press failed.", ex);
        }

        if (isDoublePress)
        {
            _settings.Store.SmartKeyDoublePressActionList = actionList;
            _settings.Store.SmartKeyDoublePressActionId = actionList[nextIndex];
        }
        else
        {
            _settings.Store.SmartKeySinglePressActionList = actionList;
            _settings.Store.SmartKeySinglePressActionId = actionList[nextIndex];
        }

        _settings.SynchronizeStore();
    }
}
