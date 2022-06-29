using System.Windows;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class NotificationsManager
    {
        private Dispatcher Dispatcher => Application.Current.Dispatcher;

        private NotificationWindow? _window;

        public NotificationsManager()
        {
            MessagingCenter.Subscribe<Notification>(this, OnNotificationReceived);
        }

        private void OnNotificationReceived(Notification notification) => Dispatcher.Invoke(() =>
        {
            var symbol = notification.Icon switch
            {
                NotificationIcon.MicrophoneOn => SymbolRegular.Mic24,
                NotificationIcon.MicrophoneOff => SymbolRegular.MicOff24,
                NotificationIcon.RefreshRate => SymbolRegular.Desktop24,
                _ => SymbolRegular.Info24,
            };

            var closeAfter = notification.Duration switch
            {
                NotificationDuration.Long => 5000,
                _ => 1000,
            };

            ShowNotification(symbol, notification.Text, closeAfter);
        });

        private void ShowNotification(SymbolRegular symbol, string text, int closeAfter)
        {
            _window?.Close();

            var nw = new NotificationWindow(symbol, text) { Owner = Application.Current.MainWindow };
            nw.Show(closeAfter);
            _window = nw;
        }
    }
}
