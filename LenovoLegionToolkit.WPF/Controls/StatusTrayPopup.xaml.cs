﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls;

public partial class StatusTrayPopup
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly BatteryFeature _batteryFeature = IoCContainer.Resolve<BatteryFeature>();
    private readonly GodModeController _godModeController = IoCContainer.Resolve<GodModeController>();
    private readonly GPUController _gpuController = IoCContainer.Resolve<GPUController>();
    private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

    private CancellationTokenSource? _cts;

    public StatusTrayPopup()
    {
        InitializeComponent();
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
        RefreshPowerMode(token);
        RefreshDiscreteGpu(token);
        RefreshBattery(token);
        RefreshUpdate(token);
    }

    private void Clear()
    {
        _powerModePresetValueLabel.Content = "-";
        _powerModePresetLabel.Visibility = Visibility.Collapsed;
        _powerModePresetValueLabel.Visibility = Visibility.Collapsed;

        _gpuValueLabel.Content = "-";
        _gpuPowerStateValueLabel.Content = "-";

        _batteryIcon.Symbol = SymbolRegular.Battery024;
        _batteryValueLabel.Content = "-";
        _batteryModeValueLabel.Content = "-";
        _batteryDischargeValueLabel.Content = "-";

        _updateIndicator.Visibility = Visibility.Collapsed;
    }

    private void RefreshPowerMode(CancellationToken token)
    {
        _powerModeFeature.GetStateAsync().ContinueWith(async t =>
        {
            if (token.IsCancellationRequested)
                return;

            if (!t.IsCompletedSuccessfully)
                return;

            var powerMode = t.Result;
            _powerModeValueLabel.Content = powerMode.GetDisplayName();

            if (powerMode != PowerModeState.GodMode)
            {
                _powerModePresetLabel.Visibility = Visibility.Collapsed;
                _powerModePresetValueLabel.Visibility = Visibility.Collapsed;
                return;
            }

            var state = await _godModeController.GetStateAsync();

            _powerModePresetValueLabel.Content = state.Presets
                .Where(p => p.Key == state.ActivePresetId)
                .Select(p => p.Value.Name)
                .FirstOrDefault();

            _powerModePresetLabel.Visibility = Visibility.Visible;
            _powerModePresetValueLabel.Visibility = Visibility.Visible;
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
            if (token.IsCancellationRequested)
                return;

            if (!t.IsCompletedSuccessfully)
                return;

            var status = t.Result;

            _gpuValueLabel.Content = status.IsActive ? "Active" : "Inactive";
            _gpuPowerStateValueLabel.Content = status.PerformanceState ?? "-";

            _gpuGrid.Visibility = Visibility.Visible;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RefreshBattery(CancellationToken token)
    {
        var batteryInformationTask = Task.Run(Battery.GetBatteryInformation);
        var batteryStateTask = _batteryFeature.GetStateAsync();

        Task.WhenAll(batteryInformationTask, batteryStateTask).ContinueWith(t =>
        {
            if (token.IsCancellationRequested)
                return;

            if (!t.IsCompletedSuccessfully)
                return;

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

            _batteryIcon.Symbol = symbol;
            _batteryValueLabel.Content = $"{batteryInfo.BatteryPercentage}%";
            _batteryModeValueLabel.Content = batteryState.GetDisplayName();
            _batteryDischargeValueLabel.Content = $"{batteryInfo.DischargeRate / 1000.0:+0.00;-0.00;0.00} W";

        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RefreshUpdate(CancellationToken token)
    {
        _updateChecker.Check().ContinueWith(t =>
        {
            if (token.IsCancellationRequested)
                return;

            if (!t.IsCompletedSuccessfully)
                return;

            var hasUpdate = t.Result is not null;
            _updateIndicator.Visibility = hasUpdate ? Visibility.Visible : Visibility.Collapsed;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}