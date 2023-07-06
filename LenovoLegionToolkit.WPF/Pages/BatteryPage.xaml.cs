﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Humanizer;
using Humanizer.Localisation;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class BatteryPage
{
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public BatteryPage()
    {
        InitializeComponent();

        IsVisibleChanged += BatteryPage_IsVisibleChanged;
    }

    private async void BatteryPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
        {
            Refresh();
            return;
        }

        _cts?.Cancel();
        if (_refreshTask is not null)
            await _refreshTask;
        _refreshTask = null;
    }

    private void Refresh()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        var token = _cts.Token;

        _refreshTask = Task.Run(async () =>
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Battery information refresh started...");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var batteryInfo = Battery.GetBatteryInformation();
                    var powerAdapterStatus = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
                    var onBatterySince = Battery.GetOnBatterySince();
                    Dispatcher.Invoke(() => Set(batteryInfo, powerAdapterStatus, onBatterySince));

                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Battery information refresh failed.", ex);
                }
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Battery information refresh stopped.");
        }, token);
    }

    private void Set(BatteryInformation batteryInfo, PowerAdapterStatus powerAdapterStatus, DateTime? onBatterySince)
    {
        var number = (int)Math.Round(batteryInfo.BatteryPercentage / 10.0);
        _batteryIcon.Symbol = number switch
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

        _percentRemaining.Text = $"{batteryInfo.BatteryPercentage}%";
        _status.Text = GetStatusText(batteryInfo);
        _lowBattery.Visibility = batteryInfo.IsLowBattery ? Visibility.Visible : Visibility.Collapsed;
        _lowWattageCharger.Visibility = powerAdapterStatus == PowerAdapterStatus.ConnectedLowWattage ? Visibility.Visible : Visibility.Collapsed;

        if (batteryInfo.BatteryTemperatureC is not null)
            _batteryTemperatureText.Text = GetTemperatureText(batteryInfo.BatteryTemperatureC);
        else
            _batteryTemperatureCardControl.Visibility = Visibility.Collapsed;

        if (!batteryInfo.IsCharging && onBatterySince.HasValue)
        {
            var onBatterySinceValue = onBatterySince.Value;
            var dateText = onBatterySinceValue.ToString("G", Resource.Culture);
            var duration = DateTime.Now.Subtract(onBatterySinceValue);
            _onBatterySinceText.Text = $"{dateText} ({duration.Humanize(2, Resource.Culture, minUnit: TimeUnit.Second)})";
        }
        else
        {
            _onBatterySinceText.Text = "-";
        }

        _batteryDischargeRateText.Text = $"{batteryInfo.DischargeRate / 1000.0:+0.00;-0.00;0.00} W";
        _batteryCapacityText.Text = $"{batteryInfo.EstimateChargeRemaining / 1000.0:0.00} Wh";
        _batteryFullChargeCapacityText.Text = $"{batteryInfo.FullChargeCapacity / 1000.0:0.00} Wh";
        _batteryDesignCapacityText.Text = $"{batteryInfo.DesignCapacity / 1000.0:0.00} Wh";

        if (batteryInfo.ManufactureDate is not null)
            _batteryManufactureDateText.Text = batteryInfo.ManufactureDate?.ToString(LocalizationHelper.ShortDateFormat) ?? "-";
        else
            _batteryManufactureDateCardControl.Visibility = Visibility.Collapsed;

        if (batteryInfo.FirstUseDate is not null)
            _batteryFirstUseDateText.Text = batteryInfo.FirstUseDate?.ToString(LocalizationHelper.ShortDateFormat) ?? "-";
        else
            _batteryFirstUseDateCardControl.Visibility = Visibility.Collapsed;

        _batteryCycleCountText.Text = $"{batteryInfo.CycleCount}";
    }

    private static string GetStatusText(BatteryInformation batteryInfo)
    {
        if (batteryInfo.IsCharging)
        {
            if (batteryInfo.DischargeRate > 0)
                return Resource.BatteryPage_ACAdapterConnectedAndCharging;

            return Resource.BatteryPage_ACAdapterConnectedNotCharging;
        }

        if (batteryInfo.BatteryLifeRemaining < 0)
            return Resource.BatteryPage_EstimatingBatteryLife;

        var time = TimeSpan.FromSeconds(batteryInfo.BatteryLifeRemaining).Humanize(2, Resource.Culture);
        return string.Format(Resource.BatteryPage_EstimatedBatteryLifeRemaining, time);
    }

    private string GetTemperatureText(double? temperature)
    {
        _batteryTemperatureCardControl.Tag = temperature;

        if (temperature is null)
            return "—";

        if (_settings.Store.TemperatureUnit == TemperatureUnit.F)
        {
            temperature *= 9.0 / 5.0;
            temperature += 32;
            return $"{temperature:0.0} {Resource.Fahrenheit}";
        }

        return $"{temperature:0.0} {Resource.Celsius}";
    }

    private void BatteryTemperatureCardControl_Click(object sender, RoutedEventArgs e)
    {
        _settings.Store.TemperatureUnit = _settings.Store.TemperatureUnit == TemperatureUnit.C ? TemperatureUnit.F : TemperatureUnit.C;
        _settings.SynchronizeStore();

        var temperature = (sender as FrameworkElement)?.Tag as double?;
        _batteryTemperatureText.Text = GetTemperatureText(temperature);
    }
}
