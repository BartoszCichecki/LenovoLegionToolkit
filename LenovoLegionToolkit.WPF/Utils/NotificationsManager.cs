using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class NotificationsManager
    {
        private Dispatcher Dispatcher => Application.Current.Dispatcher;

        private readonly ApplicationSettings _settings;

        private NotificationWindow? _window;

        public NotificationsManager(ApplicationSettings settings)
        {
            _settings = settings;

            MessagingCenter.Subscribe<Notification>(this, OnNotificationReceived);
        }

        private void OnNotificationReceived(Notification notification) => Dispatcher.Invoke(() =>
        {
            if (_settings.Store.DontShowNotifications)
                return;

            if (FullscreenHelper.IsAnyApplicationFullscreen())
                return;

            var allow = notification.Type switch
            {
                NotificationType.ACAdapterConnected => _settings.Store.Notifications.ACAdapter,
                NotificationType.ACAdapterConnectedLowWattage => _settings.Store.Notifications.ACAdapter,
                NotificationType.ACAdapterDisconnected => _settings.Store.Notifications.ACAdapter,
                NotificationType.CapsLockOn => _settings.Store.Notifications.CapsNumLock,
                NotificationType.CapsLockOff => _settings.Store.Notifications.CapsNumLock,
                NotificationType.CameraOn => _settings.Store.Notifications.CameraLock,
                NotificationType.CameraOff => _settings.Store.Notifications.CameraLock,
                NotificationType.FnLockOn => _settings.Store.Notifications.FnLock,
                NotificationType.FnLockOff => _settings.Store.Notifications.FnLock,
                NotificationType.MicrophoneOn => _settings.Store.Notifications.Microphone,
                NotificationType.MicrophoneOff => _settings.Store.Notifications.Microphone,
                NotificationType.NumLockOn => _settings.Store.Notifications.CapsNumLock,
                NotificationType.NumLockOff => _settings.Store.Notifications.CapsNumLock,
                NotificationType.PowerModeQuiet => _settings.Store.Notifications.PowerMode,
                NotificationType.PowerModeBalance => _settings.Store.Notifications.PowerMode,
                NotificationType.PowerModePerformance => _settings.Store.Notifications.PowerMode,
                NotificationType.PowerModeGodMode => _settings.Store.Notifications.PowerMode,
                NotificationType.RefreshRate => _settings.Store.Notifications.RefreshRate,
                NotificationType.RGBKeyboardBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.RGBKeyboardBacklightChanged => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.SpectrumBacklightOn => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.SpectrumBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.SpectrumBacklightPresetChanged => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.TouchpadOn => _settings.Store.Notifications.TouchpadLock,
                NotificationType.TouchpadOff => _settings.Store.Notifications.TouchpadLock,
                NotificationType.WhiteKeyboardBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
                NotificationType.WhiteKeyboardBacklightChanged => _settings.Store.Notifications.KeyboardBacklight,
                _ => throw new ArgumentException(nameof(notification.Type))
            };

            if (!allow)
                return;

            var symbol = notification.Type switch
            {
                NotificationType.ACAdapterConnected => SymbolRegular.BatteryCharge24,
                NotificationType.ACAdapterConnectedLowWattage => SymbolRegular.BatteryCharge24,
                NotificationType.ACAdapterDisconnected => SymbolRegular.BatteryCharge24,
                NotificationType.CapsLockOn => SymbolRegular.KeyboardShiftUppercase24,
                NotificationType.CapsLockOff => SymbolRegular.KeyboardShiftUppercase24,
                NotificationType.CameraOn => SymbolRegular.Camera24,
                NotificationType.CameraOff => SymbolRegular.Camera24,
                NotificationType.FnLockOn => SymbolRegular.Keyboard24,
                NotificationType.FnLockOff => SymbolRegular.Keyboard24,
                NotificationType.MicrophoneOn => SymbolRegular.Mic24,
                NotificationType.MicrophoneOff => SymbolRegular.Mic24,
                NotificationType.NumLockOn => SymbolRegular.Keyboard12324,
                NotificationType.NumLockOff => SymbolRegular.Keyboard12324,
                NotificationType.PowerModeQuiet => SymbolRegular.Gauge24,
                NotificationType.PowerModeBalance => SymbolRegular.Gauge24,
                NotificationType.PowerModePerformance => SymbolRegular.Gauge24,
                NotificationType.PowerModeGodMode => SymbolRegular.Gauge24,
                NotificationType.RefreshRate => SymbolRegular.Desktop24,
                NotificationType.RGBKeyboardBacklightOff => SymbolRegular.Lightbulb24,
                NotificationType.RGBKeyboardBacklightChanged => SymbolRegular.Lightbulb24,
                NotificationType.SpectrumBacklightOn => SymbolRegular.Lightbulb24,
                NotificationType.SpectrumBacklightOff => SymbolRegular.Lightbulb24,
                NotificationType.SpectrumBacklightPresetChanged => SymbolRegular.Lightbulb24,
                NotificationType.TouchpadOn => SymbolRegular.Tablet24,
                NotificationType.TouchpadOff => SymbolRegular.Tablet24,
                NotificationType.WhiteKeyboardBacklightOff => SymbolRegular.Lightbulb24,
                NotificationType.WhiteKeyboardBacklightChanged => SymbolRegular.Lightbulb24,
                _ => throw new ArgumentException(nameof(notification.Type))
            };

            SymbolRegular? overlaySymbol = notification.Type switch
            {
                NotificationType.ACAdapterDisconnected => SymbolRegular.Line24,
                NotificationType.CapsLockOff => SymbolRegular.Line24,
                NotificationType.CameraOff => SymbolRegular.Line24,
                NotificationType.FnLockOff => SymbolRegular.Line24,
                NotificationType.MicrophoneOff => SymbolRegular.Line24,
                NotificationType.NumLockOff => SymbolRegular.Line24,
                NotificationType.RGBKeyboardBacklightOff => SymbolRegular.Line24,
                NotificationType.SpectrumBacklightOff => SymbolRegular.Line24,
                NotificationType.TouchpadOff => SymbolRegular.Line24,
                NotificationType.WhiteKeyboardBacklightOff => SymbolRegular.Line24,
                _ => null,
            };

            var text = notification.Type switch
            {
                NotificationType.ACAdapterConnected => Resource.Notification_ACAdapterConnected,
                NotificationType.ACAdapterConnectedLowWattage => Resource.Notification_ACAdapterConnectedLowWattage,
                NotificationType.ACAdapterDisconnected => Resource.Notification_ACAdapterDisconnected,
                NotificationType.CapsLockOn => Resource.Notification_CapsLockOn,
                NotificationType.CapsLockOff => Resource.Notification_CapsLockOff,
                NotificationType.CameraOn => Resource.Notification_CameraOn,
                NotificationType.CameraOff => Resource.Notification_CameraOff,
                NotificationType.FnLockOn => Resource.Notification_FnLockOn,
                NotificationType.FnLockOff => Resource.Notification_FnLockOff,
                NotificationType.MicrophoneOn => Resource.Notification_MicrophoneOn,
                NotificationType.MicrophoneOff => Resource.Notification_MicrophoneOff,
                NotificationType.NumLockOn => Resource.Notification_NumLockOn,
                NotificationType.NumLockOff => Resource.Notification_NumLockOff,
                NotificationType.PowerModeQuiet => string.Format("{0}", notification.Args),
                NotificationType.PowerModeBalance => string.Format("{0}", notification.Args),
                NotificationType.PowerModePerformance => string.Format("{0}", notification.Args),
                NotificationType.PowerModeGodMode => string.Format("{0}", notification.Args),
                NotificationType.RefreshRate => string.Format("{0}", notification.Args),
                NotificationType.RGBKeyboardBacklightOff => string.Format("{0}", notification.Args),
                NotificationType.RGBKeyboardBacklightChanged => string.Format("{0}", notification.Args),
                NotificationType.SpectrumBacklightOn => string.Format("Backlight {0}", notification.Args),
                NotificationType.SpectrumBacklightOff => "Backlight off",
                NotificationType.SpectrumBacklightPresetChanged => string.Format("Preset {0}", notification.Args),
                NotificationType.TouchpadOn => Resource.Notification_TouchpadOn,
                NotificationType.TouchpadOff => Resource.Notification_TouchpadOff,
                NotificationType.WhiteKeyboardBacklightOff => string.Format(Resource.Notification_WhiteKeyboardBacklight, notification.Args),
                NotificationType.WhiteKeyboardBacklightChanged => string.Format(Resource.Notification_WhiteKeyboardBacklight, notification.Args),
                _ => throw new ArgumentException(nameof(notification.Type))
            };

            Action<SymbolIcon>? symbolTransform = notification.Type switch
            {
                NotificationType.PowerModeQuiet => si => si.Foreground = new SolidColorBrush(Color.FromRgb(53, 123, 242)),
                NotificationType.PowerModePerformance => si => si.Foreground = new SolidColorBrush(Color.FromRgb(212, 51, 51)),
                NotificationType.PowerModeGodMode => si => si.Foreground = new SolidColorBrush(Color.FromRgb(99, 52, 227)),
                _ => null
            };

            var closeAfter = notification.Duration switch
            {
                NotificationDuration.Long => 5000,
                _ => 1000,
            };

            if (symbolTransform is null && overlaySymbol is not null)
                symbolTransform = si => si.SetResourceReference(Control.ForegroundProperty, "TextFillColorTertiaryBrush");

            ShowNotification(symbol, overlaySymbol, symbolTransform, text, closeAfter);
        });

        private void ShowNotification(SymbolRegular symbol, SymbolRegular? overlaySymbol, Action<SymbolIcon>? symbolTransform, string text, int closeAfter)
        {
            if (_window is not null)
            {
                _window.WindowStyle = WindowStyle.None;
                _window.Close();
            }

            var nw = new NotificationWindow(symbol, overlaySymbol, symbolTransform, text) { Owner = Application.Current.MainWindow };
            nw.Show(closeAfter);
            _window = nw;
        }
    }
}
