using System.Windows;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;

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

            var symbol = notification.Type switch
            {
                NotificationType.CameraOn => SymbolRegular.Camera24,
                NotificationType.CameraOff => SymbolRegular.Camera24,
                NotificationType.MicrophoneOn => SymbolRegular.Mic24,
                NotificationType.MicrophoneOff => SymbolRegular.Mic24,
                NotificationType.RefreshRate => SymbolRegular.Desktop24,
                NotificationType.SpectrumBacklightOn => SymbolRegular.Lightbulb24,
                NotificationType.SpectrumBacklightOff => SymbolRegular.Lightbulb24,
                NotificationType.SpectrumBacklightPreset => SymbolRegular.Lightbulb24,
                NotificationType.TouchpadOn => SymbolRegular.Tablet24,
                NotificationType.TouchpadOff => SymbolRegular.Tablet24,
                _ => SymbolRegular.Info24,
            };

            SymbolRegular? overlaySymbol = notification.Type switch
            {
                NotificationType.CameraOff => SymbolRegular.Line24,
                NotificationType.MicrophoneOff => SymbolRegular.Line24,
                NotificationType.SpectrumBacklightOff => SymbolRegular.Line24,
                NotificationType.TouchpadOff => SymbolRegular.Line24,
                _ => null,
            };

            var text = notification.Type switch
            {
                NotificationType.CameraOn => Resource.Notification_CameraOn,
                NotificationType.CameraOff => Resource.Notification_CameraOff,
                NotificationType.MicrophoneOn => Resource.Notification_MicrophoneOn,
                NotificationType.MicrophoneOff => Resource.Notification_MicrophoneOff,
                NotificationType.RefreshRate => string.Format($"{0}", notification.Args),
                NotificationType.SpectrumBacklightOn => string.Format("Backlight {0}", notification.Args),
                NotificationType.SpectrumBacklightOff => "Backlight off",
                NotificationType.SpectrumBacklightPreset => string.Format("Preset {0}", notification.Args),
                NotificationType.TouchpadOn => Resource.Notification_TouchpadOn,
                NotificationType.TouchpadOff => Resource.Notification_TouchpadOff,
                _ => string.Empty,
            };

            var closeAfter = notification.Duration switch
            {
                NotificationDuration.Long => 5000,
                _ => 1000,
            };

            ShowNotification(symbol, overlaySymbol, text, closeAfter);
        });

        private void ShowNotification(SymbolRegular symbol, SymbolRegular? overlaySymbol, string text, int closeAfter)
        {
            _window?.Close();

            var nw = new NotificationWindow(symbol, overlaySymbol, text) { Owner = Application.Current.MainWindow };
            nw.Show(closeAfter);
            _window = nw;
        }
    }
}
