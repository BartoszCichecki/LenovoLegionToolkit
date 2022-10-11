using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
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
                        var batteryInfo = Battery.GetBatteryInformation();
                        var powerAdapterStatus = await Power.IsPowerAdapterConnectedAsync().ConfigureAwait(false);
                        Dispatcher.Invoke(() => Set(batteryInfo, powerAdapterStatus));

                        await Task.Delay(2000, token);
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

        private void Set(BatteryInformation batteryInfo, PowerAdapterStatus powerAdapterStatus)
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
            _lowWattageCharger.Visibility = powerAdapterStatus == PowerAdapterStatus.ConnectedLowWattage ? Visibility.Visible : Visibility.Hidden;

            if (batteryInfo.BatteryTemperatureC is not null)
                _batteryTemperatureText.Text = GetTemperatureText(batteryInfo.BatteryTemperatureC);
            else
                _batteryTemperatureCardControl.Visibility = Visibility.Collapsed;

            if (!batteryInfo.IsCharging && batteryInfo.OnBatterySince.HasValue)
            {
                var onBatterySince = batteryInfo.OnBatterySince.Value;
                var dateText = onBatterySince.ToString("G");
                var duration = DateTime.Now.Subtract(onBatterySince);
                _onBatterySinceText.Text = $"{dateText} ({duration.Hours}h {duration.Minutes}m {duration.Seconds}s)";
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
                _batteryManufactureDateText.Text = batteryInfo.ManufactureDate?.ToString("d") ?? "-";
            else
                _batteryManufactureDateCardControl.Visibility = Visibility.Collapsed;

            if (batteryInfo.FirstUseDate is not null)
                _batteryFirstUseDateText.Text = batteryInfo.FirstUseDate?.ToString("d") ?? "-";
            else
                _batteryFirstUseDateCardControl.Visibility = Visibility.Collapsed;

            _batteryCycleCountText.Text = $"{batteryInfo.CycleCount}";
        }

        private string GetStatusText(BatteryInformation batteryInfo)
        {
            if (batteryInfo.IsCharging)
            {
                if (batteryInfo.DischargeRate > 0)
                    return Resource.BatteryPage_ACAdapterConnectedAndCharging;

                return Resource.BatteryPage_ACAdapterConnectedNotCharging;
            }

            if (batteryInfo.BatteryLifeRemaining < 0)
                return Resource.BatteryPage_EstimatingBatteryLife;

            return string.Format(Resource.BatteryPage_EstimatedBatteryLifeRemaining, GetTimeString(batteryInfo.BatteryLifeRemaining));
        }

        private static string GetTimeString(int seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
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
