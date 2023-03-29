﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;
using Application = System.Windows.Application;

namespace LenovoLegionToolkit.WPF.Controls;

public partial class StatusTrayPopup
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly BatteryFeature _batteryFeature = IoCContainer.Resolve<BatteryFeature>();
    private readonly GodModeController _godModeController = IoCContainer.Resolve<GodModeController>();
    private readonly GPUController _gpuController = IoCContainer.Resolve<GPUController>();
    private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();
    private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

    private CancellationTokenSource? _cts;

    public StatusTrayPopup()
    {
        InitializeComponent();

        _themeManager.ThemeApplied += (_, _) => Resources = Application.Current.Resources;

#if DEBUG
        _title.Text += " [DEBUG]";
#else
        var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
        if (version == new Version(0, 0, 1, 0) || version?.Build == 99)
            _title.Text += " [BETA]";
#endif

        if (Log.Instance.IsTraceEnabled)
            _title.Text += " [LOGGING ENABLED]";
    }

    private void StatusTrayPopup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _cts?.Cancel();

        if (!IsVisible)
        {
            Clear();
            return;
        }

        _cts = new();
        Refresh(_cts.Token);
    }

    private void Refresh(CancellationToken token)
    {
        try
        {
            RefreshPowerMode(token);
            RefreshDiscreteGpu(token);
            RefreshBattery(token);
            RefreshUpdate(token);
        }
        catch (TaskCanceledException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Error refreshing popup.", ex);
        }
    }

    private void Clear()
    {
        _powerModePresetValueLabel.Content = null;
        _powerModeValueIndicator.Fill = null;
        _powerModePresetLabel.Visibility = Visibility.Collapsed;
        _powerModePresetValueLabel.Visibility = Visibility.Collapsed;

        _gpuPowerStateValueLabel.Content = null;
        _gpuActive.Visibility = Visibility.Collapsed;
        _gpuInactive.Visibility = Visibility.Collapsed;

        _batteryIcon.Symbol = SymbolRegular.Battery024;
        _batteryValueLabel.Content = null;
        _batteryValueLabel.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");
        _batteryModeValueLabel.Content = null;
        _batteryDischargeValueLabel.Content = null;

        _updateIndicator.Visibility = Visibility.Collapsed;
    }

    private void RefreshPowerMode(CancellationToken token)
    {
        _powerModeFeature.GetStateAsync().ContinueWith(async t =>
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                if (!t.IsCompletedSuccessfully)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Failed to refresh Power Mode.", t.Exception?.InnerException);
                    return;
                }

                var powerMode = t.Result;
                _powerModeValueLabel.Content = powerMode.GetDisplayName();
                _powerModeValueIndicator.Fill = new SolidColorBrush(powerMode switch
                {
                    PowerModeState.Quiet => Color.FromRgb(53, 123, 242),
                    PowerModeState.Performance => Color.FromRgb(212, 51, 51),
                    PowerModeState.GodMode => Color.FromRgb(99, 52, 227),
                    _ => Colors.White
                });

                if (powerMode != PowerModeState.GodMode)
                {
                    _powerModePresetLabel.Visibility = Visibility.Collapsed;
                    _powerModePresetValueLabel.Visibility = Visibility.Collapsed;
                    return;
                }

                _powerModePresetValueLabel.Content = await _godModeController.GetActivePresetNameAsync() ?? "-";

                _powerModePresetLabel.Visibility = Visibility.Visible;
                _powerModePresetValueLabel.Visibility = Visibility.Visible;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error continuing on power mode refresh.", ex);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RefreshDiscreteGpu(CancellationToken token)
    {
        if (!_gpuController.IsSupported())
        {
            _gpuGrid.Visibility = Visibility.Collapsed;
            return;
        }

        _gpuController.RefreshNowAsync().ContinueWith(t =>
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                if (!t.IsCompletedSuccessfully)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Failed to refresh Discrete GPU.", t.Exception?.InnerException);
                    return;
                }

                var status = t.Result;

                if (status.IsActive)
                {
                    _gpuPowerStateValueLabel.Content = status.PerformanceState ?? "-";

                    _gpuActive.Visibility = Visibility.Visible;
                    _gpuInactive.Visibility = Visibility.Collapsed;
                    _gpuPoweredOff.Visibility = Visibility.Collapsed;
                    _gpuPowerStateValue.Visibility = Visibility.Visible;
                    _gpuPowerStateValueLabel.Visibility = Visibility.Visible;
                }
                else if (status.IsPoweredOff)
                {
                    _gpuPowerStateValueLabel.Content = null;

                    _gpuActive.Visibility = Visibility.Collapsed;
                    _gpuInactive.Visibility = Visibility.Collapsed;
                    _gpuPoweredOff.Visibility = Visibility.Visible;
                    _gpuPowerStateValue.Visibility = Visibility.Collapsed;
                    _gpuPowerStateValueLabel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _gpuPowerStateValueLabel.Content = status.PerformanceState ?? "-";

                    _gpuActive.Visibility = Visibility.Collapsed;
                    _gpuInactive.Visibility = Visibility.Visible;
                    _gpuPoweredOff.Visibility = Visibility.Collapsed;
                    _gpuPowerStateValue.Visibility = Visibility.Visible;
                    _gpuPowerStateValueLabel.Visibility = Visibility.Visible;
                }

                _gpuGrid.Visibility = Visibility.Visible;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error continuing on GPU refresh.", ex);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RefreshBattery(CancellationToken token)
    {
        var batteryInformationTask = Task.Run(Battery.GetBatteryInformation, token);
        var batteryStateTask = _batteryFeature.GetStateAsync();

        Task.WhenAll(batteryInformationTask, batteryStateTask).ContinueWith(t =>
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                if (!t.IsCompletedSuccessfully)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Failed to refresh battery.", t.Exception?.InnerException);
                    return;
                }

                var batteryInfo = batteryInformationTask.Result;
                var batteryState = batteryStateTask.Result;

                var symbol = (int)Math.Round(batteryInfo.BatteryPercentage / 10.0) switch
                {
                    10 => SymbolRegular.Battery1024,
                    9 => SymbolRegular.Battery924,
                    8 => SymbolRegular.Battery824,
                    7 => SymbolRegular.Battery724,
                    6 => SymbolRegular.Battery624,
                    5 => SymbolRegular.Battery524,
                    4 => SymbolRegular.Battery424,
                    3 => SymbolRegular.Battery324,
                    2 => SymbolRegular.Battery224,
                    1 => SymbolRegular.Battery124,
                    _ => SymbolRegular.Battery024,
                };

                if (batteryInfo.IsCharging)
                    symbol = batteryState == BatteryState.Conservation ? SymbolRegular.BatterySaver24 : SymbolRegular.BatteryCharge24;

                if (batteryInfo.IsLowBattery)
                    _batteryValueLabel.SetResourceReference(ForegroundProperty, "SystemFillColorCautionBrush");

                _batteryIcon.Symbol = symbol;
                _batteryValueLabel.Content = $"{batteryInfo.BatteryPercentage}%";
                _batteryModeValueLabel.Content = batteryState.GetDisplayName();
                _batteryDischargeValueLabel.Content = $"{batteryInfo.DischargeRate / 1000.0:+0.00;-0.00;0.00} W";
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error continuing on battery refresh.", ex);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RefreshUpdate(CancellationToken token)
    {
        _updateChecker.Check().ContinueWith(t =>
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                if (!t.IsCompletedSuccessfully)
                    return;

                var hasUpdate = t.Result is not null;
                _updateIndicator.Visibility = hasUpdate ? Visibility.Visible : Visibility.Collapsed;
                _updateIndicator.Visibility = Visibility.Collapsed;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error continuing on update refresh.", ex);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}