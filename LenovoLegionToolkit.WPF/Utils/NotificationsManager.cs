using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils;

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
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Notification {notification} received");

        if (_settings.Store.DontShowNotifications)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Notifications are disabled.");

            return;
        }

        if (FullscreenHelper.IsAnyApplicationFullscreen())
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Some application is in fullscreen.");

            return;
        }

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
            NotificationType.PanelLogoLightingOn => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.PanelLogoLightingOff => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.PortLightingOn => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.PortLightingOff => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.PowerModeQuiet => _settings.Store.Notifications.PowerMode,
            NotificationType.PowerModeBalance => _settings.Store.Notifications.PowerMode,
            NotificationType.PowerModePerformance => _settings.Store.Notifications.PowerMode,
            NotificationType.PowerModeGodMode => _settings.Store.Notifications.PowerMode,
            NotificationType.RefreshRate => _settings.Store.Notifications.RefreshRate,
            NotificationType.RGBKeyboardBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.RGBKeyboardBacklightChanged => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.SmartKeyDoublePress => _settings.Store.Notifications.SmartKey,
            NotificationType.SmartKeySinglePress => _settings.Store.Notifications.SmartKey,
            NotificationType.SpectrumBacklightChanged => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.SpectrumBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.SpectrumBacklightPresetChanged => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.TouchpadOn => _settings.Store.Notifications.TouchpadLock,
            NotificationType.TouchpadOff => _settings.Store.Notifications.TouchpadLock,
            NotificationType.UpdateAvailable => _settings.Store.Notifications.UpdateAvailable,
            NotificationType.WhiteKeyboardBacklightOff => _settings.Store.Notifications.KeyboardBacklight,
            NotificationType.WhiteKeyboardBacklightChanged => _settings.Store.Notifications.KeyboardBacklight,
            _ => throw new ArgumentException(nameof(notification.Type))
        };

        if (!allow)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Notification type {notification.Type} is disabled.");

            return;
        }

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
            NotificationType.PanelLogoLightingOn => SymbolRegular.Lightbulb24,
            NotificationType.PanelLogoLightingOff => SymbolRegular.Lightbulb24,
            NotificationType.PortLightingOn => SymbolRegular.Lightbulb24,
            NotificationType.PortLightingOff => SymbolRegular.Lightbulb24,
            NotificationType.PowerModeQuiet => SymbolRegular.Gauge24,
            NotificationType.PowerModeBalance => SymbolRegular.Gauge24,
            NotificationType.PowerModePerformance => SymbolRegular.Gauge24,
            NotificationType.PowerModeGodMode => SymbolRegular.Gauge24,
            NotificationType.RefreshRate => SymbolRegular.DesktopPulse24,
            NotificationType.RGBKeyboardBacklightOff => SymbolRegular.Lightbulb24,
            NotificationType.RGBKeyboardBacklightChanged => SymbolRegular.Lightbulb24,
            NotificationType.SmartKeyDoublePress => SymbolRegular.StarEmphasis24,
            NotificationType.SmartKeySinglePress => SymbolRegular.Star24,
            NotificationType.SpectrumBacklightChanged => SymbolRegular.Lightbulb24,
            NotificationType.SpectrumBacklightOff => SymbolRegular.Lightbulb24,
            NotificationType.SpectrumBacklightPresetChanged => SymbolRegular.Lightbulb24,
            NotificationType.TouchpadOn => SymbolRegular.Tablet24,
            NotificationType.TouchpadOff => SymbolRegular.Tablet24,
            NotificationType.UpdateAvailable => SymbolRegular.ArrowSync24,
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
            NotificationType.PanelLogoLightingOff => SymbolRegular.Line24,
            NotificationType.PortLightingOff => SymbolRegular.Line24,
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
            NotificationType.PanelLogoLightingOn => Resource.Notification_PanelLogoLightingOn,
            NotificationType.PanelLogoLightingOff => Resource.Notification_PanelLogoLightingOff,
            NotificationType.PortLightingOn => Resource.Notification_PortLightingOn,
            NotificationType.PortLightingOff => Resource.Notification_PortLightingOff,
            NotificationType.PowerModeQuiet => string.Format("{0}", notification.Args),
            NotificationType.PowerModeBalance => string.Format("{0}", notification.Args),
            NotificationType.PowerModePerformance => string.Format("{0}", notification.Args),
            NotificationType.PowerModeGodMode => string.Format("{0}", notification.Args),
            NotificationType.RefreshRate => string.Format("{0}", notification.Args),
            NotificationType.RGBKeyboardBacklightOff => string.Format("{0}", notification.Args),
            NotificationType.RGBKeyboardBacklightChanged => string.Format("{0}", notification.Args),
            NotificationType.SmartKeyDoublePress => string.Format("{0}", notification.Args),
            NotificationType.SmartKeySinglePress => string.Format("{0}", notification.Args),
            NotificationType.SpectrumBacklightChanged => string.Format(Resource.Notification_SpectrumKeyboardBacklight_Brightness, notification.Args),
            NotificationType.SpectrumBacklightOff => string.Format(Resource.Notification_SpectrumKeyboardBacklight_Backlight, notification.Args),
            NotificationType.SpectrumBacklightPresetChanged => string.Format(Resource.Notification_SpectrumKeyboardBacklight_Profile, notification.Args),
            NotificationType.TouchpadOn => Resource.Notification_TouchpadOn,
            NotificationType.TouchpadOff => Resource.Notification_TouchpadOff,
            NotificationType.UpdateAvailable => string.Format(Resource.Notification_UpdateAvailable, notification.Args),
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
            _ => 1000
        };

        Action? clickAction = notification.Type switch
        {
            NotificationType.UpdateAvailable => UpdateAvailableAction,
            _ => null
        };

        if (symbolTransform is null && overlaySymbol is not null)
            symbolTransform = si => si.SetResourceReference(Control.ForegroundProperty, "TextFillColorSecondaryBrush");

        ShowNotification(symbol, overlaySymbol, symbolTransform, text, closeAfter, clickAction);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Notification {notification} shown.");
    });

    private void ShowNotification(SymbolRegular symbol, SymbolRegular? overlaySymbol, Action<SymbolIcon>? symbolTransform, string text, int closeAfter, Action? clickAction)
    {
        var mainWindow = Application.Current.MainWindow;
        if (mainWindow is null)
            return;

        if (_window is not null)
        {
            _window.WindowStyle = WindowStyle.None;
            _window.Close();
        }

        var nw = new NotificationWindow(symbol, overlaySymbol, symbolTransform, text, clickAction, _settings.Store.NotificationPosition) { Owner = mainWindow };
        nw.Show(closeAfter);
        _window = nw;
    }

    private static void UpdateAvailableAction()
    {
        if (App.Current.MainWindow is not MainWindow mainWindow) return;

        mainWindow.BringToForeground();
        mainWindow.ShowUpdateWindow();
    }
}