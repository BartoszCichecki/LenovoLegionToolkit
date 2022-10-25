using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Settings;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Settings
{
    public partial class NotificationsSettingsWindow
    {
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

        private CardControl[] Cards => new[]
        {
            _capsNumLockCard,
            _fnLockCard,
            _touchpadLockCard,
            _cameraLockCard,
            _powerModeCard,
            _refreshRateCard,
            _microphoneCard,
            _keyboardBacklightCard
        };

        public NotificationsSettingsWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            _dontShowNotificationsToggle.IsChecked = _settings.Store.DontShowNotifications;

            _capsNumLockToggle.IsChecked = _settings.Store.Notifications.CapsNumLock;
            _fnLockToggle.IsChecked = _settings.Store.Notifications.FnLock;
            _touchpadLockToggle.IsChecked = _settings.Store.Notifications.TouchpadLock;
            _cameraLockToggle.IsChecked = _settings.Store.Notifications.CameraLock;
            _powerModeToggle.IsChecked = _settings.Store.Notifications.PowerMode;
            _refreshRateToggle.IsChecked = _settings.Store.Notifications.RefreshRate;
            _microphoneToggle.IsChecked = _settings.Store.Notifications.Microphone;
            _keyboardBacklightToggle.IsChecked = _settings.Store.Notifications.KeyboardBacklight;

            RefreshCards();
        }

        private void RefreshCards()
        {
            var notificationsDisabled = _dontShowNotificationsToggle.IsChecked ?? false;

            foreach (var card in Cards)
                card.IsEnabled = !notificationsDisabled;
        }

        private void DontShowNotificationsToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _dontShowNotificationsToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.DontShowNotifications = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void CapsNumLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _capsNumLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.CapsNumLock = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void FnLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _fnLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.FnLock = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void TouchpadLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _touchpadLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.TouchpadLock = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void CameraLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _cameraLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.CameraLock = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void PowerModeToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _powerModeToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.PowerMode = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void RefreshRateToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _refreshRateToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.RefreshRate = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void MicrophoneToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _microphoneToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.Microphone = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }

        private void KeyboardBacklightToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _keyboardBacklightToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.KeyboardBacklight = state.Value;
            _settings.SynchronizeStore();

            RefreshCards();
        }
    }
}
