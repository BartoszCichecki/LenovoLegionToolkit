using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using Wpf.Ui.Common;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class BatteryPage
    {
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

            if (batteryInfo.IsCharging)
            {
                if (batteryInfo.DischargeRate > 0)
                    _status.Text = "Connected, charging...";
                else
                    _status.Text = $"Connected, not charging";
            }
            else
            {
                if (batteryInfo.BatteryLifeRemaining < 0)
                    _status.Text = "Estimating time...";
                else
                {
                    var timeSpan = TimeSpan.FromSeconds(batteryInfo.BatteryLifeRemaining);
                    _status.Text = $"Estimated time remaining: {GetTimeString(timeSpan)}";
                }
            }

            _batteryTemperatureText.Text = batteryInfo.BatteryTemperatureC is null ? "—" : $"{batteryInfo.BatteryTemperatureC:0.0} °C";

            _batteryDischargeRateText.Text = $"{batteryInfo.DischargeRate / 1000.0:0.00} Wh";
            _batteryCapacityText.Text = $"{batteryInfo.EstimateChargeRemaining / 1000.0:0.00} Wh";
            _batteryFullChargeCapacityText.Text = $"{batteryInfo.FullChargeCapactiy / 1000.0:0.00} Wh";
            _batteryDesignCapacityText.Text = $"{batteryInfo.DesignCapacity / 1000.0:0.00} Wh";

            _batteryCycleCountText.Text = $"{batteryInfo.CycleCount}";
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
    }
}
