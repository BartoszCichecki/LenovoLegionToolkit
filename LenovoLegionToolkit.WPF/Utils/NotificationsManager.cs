using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class NotificationsManager
    {
        private readonly SpecialKeyListener _specialKeyListener;
        private readonly DriverKeyListener _driverKeyListener;
        private readonly RefreshRateFeature _refreshRateFeature;

        private Dispatcher Dispatcher => Application.Current.Dispatcher;

        private NotificationWindow? _window;

        public NotificationsManager(SpecialKeyListener specialKeyListener,
                                    DriverKeyListener driverKeyListener,
                                    RefreshRateFeature refreshRateFeature)
        {
            _specialKeyListener = specialKeyListener;
            _driverKeyListener = driverKeyListener;
            _refreshRateFeature = refreshRateFeature;

            _specialKeyListener.Changed += SpecialKeyListener_Changed;
            _driverKeyListener.Changed += DriverKeyListener_Changed;
        }

        private void DriverKeyListener_Changed(object? sender, DriverKey e) => Dispatcher.Invoke(() =>
        {
            try
            {
                if (e == DriverKey.Fn_F4)
                {
                    if (Microphone.IsEnabled)
                        ShowNotification(SymbolRegular.Mic24, "Microphone on");
                    else
                        ShowNotification(SymbolRegular.MicOff24, "Microphone muted");
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to show notification: {ex.Demystify()}");
            }
        });

        private void SpecialKeyListener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            try
            {
                if (e == SpecialKey.Fn_R)
                {
                    var allStates = await _refreshRateFeature.GetAllStatesAsync();
                    var state = await _refreshRateFeature.GetStateAsync();
                    if (allStates.Length > 0 && state != default)
                        ShowNotification(SymbolRegular.Desktop24, state.DisplayName, 5000);
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to show notification: {ex.Demystify()}");
            }
        });

        private void ShowNotification(SymbolRegular symbol, string text, int closeAfter = 1000)
        {
            _window?.Close();

            var nw = new NotificationWindow(symbol, text) { Owner = Application.Current.MainWindow };
            nw.Show(closeAfter);
            _window = nw;
        }
    }
}
