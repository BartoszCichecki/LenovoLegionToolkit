using System.Windows;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
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

            var symbol = notification.Icon switch
            {
                NotificationIcon.MicrophoneOn => SymbolRegular.Mic24,
                NotificationIcon.MicrophoneOff => SymbolRegular.Mic24,
                NotificationIcon.RefreshRate => SymbolRegular.Desktop24,
                NotificationIcon.TouchpadOn => SymbolRegular.Tablet24,
                NotificationIcon.TouchpadOff => SymbolRegular.Tablet24,
                _ => SymbolRegular.Info24,
            };

            SymbolRegular? overlaySymbol = notification.Icon switch
            {
                NotificationIcon.MicrophoneOff => SymbolRegular.Line24,
                NotificationIcon.TouchpadOff => SymbolRegular.Line24,
                _ => null,
            };

            var closeAfter = notification.Duration switch
            {
                NotificationDuration.Long => 5000,
                _ => 1000,
            };

            ShowNotification(symbol, overlaySymbol, notification.Text, closeAfter);
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
