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
            _keyboardBacklightCard,
            _cameraLockCard,
            _microphoneCard,
            _powerModeCard,
            _refreshRateCard,
            _acAdapterCard,
            _smartKeySinglePressCard,
            _smartKeyDoublePressCard
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
            _keyboardBacklightToggle.IsChecked = _settings.Store.Notifications.KeyboardBacklight;
            _cameraLockToggle.IsChecked = _settings.Store.Notifications.CameraLock;
            _microphoneToggle.IsChecked = _settings.Store.Notifications.Microphone;
            _powerModeToggle.IsChecked = _settings.Store.Notifications.PowerMode;
            _refreshRateToggle.IsChecked = _settings.Store.Notifications.RefreshRate;
            _acAdapterToggle.IsChecked = _settings.Store.Notifications.ACAdapter;
            _smartKeySinglePressToggle.IsChecked = _settings.Store.Notifications.SmartKeySinglePress;
            _smartKeyDoublePressToggle.IsChecked = _settings.Store.Notifications.SmartKeyDoublePress;

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
        }

        private void FnLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _fnLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.FnLock = state.Value;
            _settings.SynchronizeStore();
        }

        private void TouchpadLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _touchpadLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.TouchpadLock = state.Value;
            _settings.SynchronizeStore();
        }

        private void KeyboardBacklightToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _keyboardBacklightToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.KeyboardBacklight = state.Value;
            _settings.SynchronizeStore();
        }

        private void CameraLockToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _cameraLockToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.CameraLock = state.Value;
            _settings.SynchronizeStore();
        }

        private void MicrophoneToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _microphoneToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.Microphone = state.Value;
            _settings.SynchronizeStore();
        }

        private void PowerModeToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _powerModeToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.PowerMode = state.Value;
            _settings.SynchronizeStore();
        }

        private void RefreshRateToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _refreshRateToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.RefreshRate = state.Value;
            _settings.SynchronizeStore();
        }

        private void ACAdapterToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _acAdapterToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.ACAdapter = state.Value;
            _settings.SynchronizeStore();
        }

        private void SmartKeySinglePressToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _smartKeySinglePressToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.SmartKeySinglePress = state.Value;
            _settings.SynchronizeStore();
        }

        private void SmartKeyDoublePressToggle_Click(object sender, RoutedEventArgs e)
        {
            var state = _smartKeyDoublePressToggle.IsChecked;
            if (state is null)
                return;

            _settings.Store.Notifications.SmartKeyDoublePress = state.Value;
            _settings.SynchronizeStore();
        }
    }
}
