using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#pragma warning disable IDE1006 // Naming Styles

namespace LenovoLegionToolkit
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class FeatureCheck
        {
            private readonly Action _check;
            private readonly Action _disable;

            public FeatureCheck(Action check, Action disable)
            {
                _check = check;
                _disable = disable;
            }

            public void Check() => _check();
            public void Disable() => _disable();
        }

        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new();
        private readonly BatteryFeature _batteryFeature = new();
        private readonly FlipToStartFeature _flipToStartFeature = new();
        private readonly FnLockFeature _fnLockFeature = new();
        private readonly HybridModeFeature _hybridModeFeature = new();
        private readonly OverDriveFeature _overDriveFeature = new();
        private readonly PowerModeFeature _powerModeFeature = new();
        private readonly TouchpadLockFeature _touchpadLockFeature = new();

        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly RadioButton[] _batteryButtons;
        private readonly RadioButton[] _hybridModeButtons;
        private readonly RadioButton[] _powerModeButtons;

        private readonly Autorun _autorun = new();
        private readonly UpdateChecker _updateChecker = new();
        private readonly NVidiaMonitor _nvidiaMonitor = new();

        public MainWindow()
        {
            InitializeComponent();

            StateChanged += mainWindow_StateChanged;
            _nvidiaMonitor.Refreshed += nvidiaMonitor_Refreshed;
            ((App)Application.Current).PowerModeListener.Changed += powerModeListener_Changed;

            autorunMenuItem.IsChecked = _autorun.IsEnabled;

            _alwaysOnUsbButtons = new[] { radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways };
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            _hybridModeButtons = new[] { radioHybridOn, radioHybridOff };
            _powerModeButtons = new[] { radioQuiet, radioBalance, radioPerformance };

            updateIndicator.Visibility = Visibility.Collapsed;

            Refresh();
            CheckUpdates();
        }

        public void BringToForeground()
        {
            ShowInTaskbar = true;

            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();

            Refresh();
        }

        public void SendToTray()
        {
            Hide();
            ShowInTaskbar = false;
        }

        private void CheckUpdates()
        {
            Task.Run(_updateChecker.CheckUpdates)
                .ContinueWith(updatesAvailable =>
            {
                updateIndicator.Visibility = updatesAvailable.Result ? Visibility.Visible : Visibility.Collapsed;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Refresh()
        {
            var features = new[]
            {
                new FeatureCheck(
                    () => _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true,
                    () => DisableControls(_alwaysOnUsbButtons)
                ),
                new FeatureCheck(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableControls(_batteryButtons)
                ),
                new FeatureCheck(
                    () => chkFlipToStart.IsChecked = _flipToStartFeature.GetState() == FlipToStartState.On,
                    () => chkFlipToStart.IsEnabled = false
                ),
                new FeatureCheck(
                    () => chkFnLock.IsChecked = _fnLockFeature.GetState() == FnLockState.On,
                    () => chkFnLock.IsEnabled = false
                ),
                new FeatureCheck(
                    () => _hybridModeButtons[(int) _hybridModeFeature.GetState()].IsChecked = true,
                    () => DisableControls(_hybridModeButtons)
               ),
                new FeatureCheck(
                    () => chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On,
                    () => chkOverDrive.IsEnabled = false
                ),
                new FeatureCheck(
                    () => _powerModeButtons[(int) _powerModeFeature.GetState()].IsChecked = true,
                    () => DisableControls(_powerModeButtons)
                ),
                new FeatureCheck(
                    () => chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On,
                    () => chkTouchpadLock.IsEnabled = false
                ),
            };

            foreach (var feature in features)
            {
                try { feature.Check(); }
                catch { feature.Disable(); }
            }
        }

        private static void DisableControls(Control[] buttons)
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    SendToTray();
                    _nvidiaMonitor.Stop();
                    break;
                case WindowState.Normal:
                    BringToForeground();
                    _nvidiaMonitor.Start();
                    break;
            }
        }

        private void nvidiaMonitor_Refreshed(object sender, NVidiaMonitor.RefreshedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                lblDiscreteGPUStatusValue.Content = e.IsActive ? "Active" : "Inactive";
                lblDiscreteGPUStatusValue.ToolTip = e.Status switch
                {
                    NVidiaMonitor.Status.SingleVideoCardMode => "There is only one video card.",
                    NVidiaMonitor.Status.DiscreteGPUNotFound => "Discrete nVidia GPU not found.",
                    NVidiaMonitor.Status.MonitorsConnected => "Monitor is connected to nVidia GPU.",
                    _ => null,

                };
                lblDiscreteGPUProcessesValue.Content = e.ProcessCount < 0 ? "-" : e.ProcessCount.ToString();
                btnDiscreteExternalGPU.IsEnabled = e.CanBeDisabled;
            });
        }

        public void powerModeListener_Changed(object sender, PowerModeState state)
        {
            Dispatcher.Invoke(() => { _powerModeButtons[(int)state].IsChecked = true; });
        }

        private void radioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (PowerModeState)Array.IndexOf(_powerModeButtons, sender);
            if (_powerModeFeature.GetState() == state)
                return;
            _powerModeFeature.SetState(state);
        }

        private void hybridMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (HybridModeState)Array.IndexOf(_hybridModeButtons, sender);
            if (_hybridModeFeature.GetState() == state)
                return;
            _hybridModeFeature.SetState(state);

            var result = MessageBox.Show("Changing Hybrid Mode requires restart. Do you want to restart now?",
                "Restart required",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (result != MessageBoxResult.Yes)
            {
                Refresh();
                return;
            }

            OS.Restart();
        }

        private void radioBattery_Checked(object sender, RoutedEventArgs e)
        {
            var state = (BatteryState)Array.IndexOf(_batteryButtons, sender);
            if (_batteryFeature.GetState() == state)
                return;
            _batteryFeature.SetState(state);
        }

        private void radioAlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            var state = (AlwaysOnUsbState)Array.IndexOf(_alwaysOnUsbButtons, sender);
            if (_alwaysOnUsbFeature.GetState() == state)
                return;
            _alwaysOnUsbFeature.SetState(state);
        }

        private void chkFlipToStart_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFlipToStart.IsChecked.GetValueOrDefault(false)
                ? FlipToStartState.On
                : FlipToStartState.Off;
            if (_flipToStartFeature.GetState() == state)
                return;
            _flipToStartFeature.SetState(state);
        }

        private void chkOverDrive_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkOverDrive.IsChecked.GetValueOrDefault(false)
                ? OverDriveState.On
                : OverDriveState.Off;
            if (_overDriveFeature.GetState() == state)
                return;
            _overDriveFeature.SetState(state);
        }

        private void chkFnLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFnLock.IsChecked.GetValueOrDefault(false)
                ? FnLockState.On
                : FnLockState.Off;
            if (_fnLockFeature.GetState() == state)
                return;
            _fnLockFeature.SetState(state);
        }

        private void chkTouchpadLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkTouchpadLock.IsChecked.GetValueOrDefault(false)
                ? TouchpadLockState.On
                : TouchpadLockState.Off;
            if (_touchpadLockFeature.GetState() == state)
                return;
            _touchpadLockFeature.SetState(state);
        }

        private void notifyIcon_TrayMouseUp(object sender, RoutedEventArgs e) => BringToForeground();

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e) => new AboutWindow { Owner = this }.ShowDialog();

        private void autorunMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_autorun.IsEnabled)
                _autorun.Disable();
            else
            {
                var result = MessageBox.Show("If you move Lenovo Legion Toolkit to a different location after enabling this option, please disable and enable this option to keep it starting automatically.",
                    "Run on startup",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (result != MessageBoxResult.OK)
                    return;

                _autorun.Enable();
            }

            autorunMenuItem.IsChecked = _autorun.IsEnabled;
        }

        private void enableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Enable();

            var result = MessageBox.Show("It is recommended to restart Windows after enabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            OS.Restart();
        }

        private void disableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Disable();

            var result = MessageBox.Show("It is recommended to restart Windows after disabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            OS.Restart();
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}