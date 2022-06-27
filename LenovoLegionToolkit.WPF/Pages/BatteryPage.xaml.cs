using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Wpf.Ui.Common;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class BatteryPage
    {
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

        private CancellationTokenSource? _cts;
        private Task? _refreshTask;

        public BatteryPage()
        {
            InitializeComponent();

            Loaded += BatteryPage_Loaded;
            IsVisibleChanged += BatteryPage_IsVisibleChanged;
        }

        private void BatteryPage_Loaded(object sender, RoutedEventArgs e) => Refresh();

        private void BatteryPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (IsVisible)
                Refresh();
            else
                _cts?.Cancel();
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
                        var batteryInfo = await Battery.GetBatteryInformationAsync();
                        Dispatcher.Invoke(() => Set(batteryInfo));

                        await Task.Delay(1000, token);
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Battery information refresh failed: {ex.Demystify()}");
                    }
                }

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Battery information refresh stopped.");
            }, token);
        }

        private void Set(BatteryInformation batteryInfo)
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

            _precentRemaining.Text = $"{batteryInfo.BatteryPercentage}%";
            _status.Text = GetStatusText(batteryInfo);
            _batteryTemperatureText.Text = GetTemperatureText(batteryInfo.BatteryTemperatureC);
            _batteryDischargeRateText.Text = $"{batteryInfo.DischargeRate / 1000.0:0.00} W";
            _batteryCapacityText.Text = $"{batteryInfo.EstimateChargeRemaining / 1000.0:0.00} Wh";
            _batteryFullChargeCapacityText.Text = $"{batteryInfo.FullChargeCapactiy / 1000.0:0.00} Wh";
            _batteryDesignCapacityText.Text = $"{batteryInfo.DesignCapacity / 1000.0:0.00} Wh";
            _batteryManufactureDateText.Text = batteryInfo.ManufactureDate?.ToString("d") ?? "-";

            _batteryCycleCountText.Text = $"{batteryInfo.CycleCount}";
        }

        private string GetStatusText(BatteryInformation batteryInfo)
        {
            if (batteryInfo.IsCharging)
            {
                if (batteryInfo.DischargeRate > 0)
                    return "Connected, charging...";
                else
                    return $"Connected, not charging";
            }
            else
            {
                if (batteryInfo.BatteryLifeRemaining < 0)
                    return "Estimating time...";
                else
                {
                    var timeSpan = TimeSpan.FromSeconds(batteryInfo.BatteryLifeRemaining);
                    return $"Estimated time remaining: {GetTimeString(timeSpan)}";
                }
            }
        }

        private static string GetTimeString(TimeSpan timeSpan)
        {
            var result = string.Empty;

            var hours = timeSpan.Hours;
            if (hours > 0)
                result += $"{hours} hour{(hours == 1 ? "" : "s")} ";

            var minutes = timeSpan.Minutes;
            result += $"{minutes} minute{(minutes == 1 ? "" : "s")}";

            return result;
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
                return $"{temperature:0.0} °F";
            }


            return $"{temperature:0.0} °C";
        }

        private void BatteryTemperatureCardControl_Click(object sender, RoutedEventArgs e)
        {
            _settings.Store.TemperatureUnit = _settings.Store.TemperatureUnit == TemperatureUnit.C ? TemperatureUnit.F : TemperatureUnit.C;
            _settings.SynchronizeStore();

            var temperature = (sender as FrameworkElement)?.Tag as double?;
            _batteryTemperatureText.Text = GetTemperatureText(temperature);
        }
    }
}
