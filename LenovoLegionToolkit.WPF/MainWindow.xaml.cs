using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly RadioButton[] _batteryButtons;
        private readonly RadioButton[] _hybridModeButtons;
        private readonly RadioButton[] _powerModeButtons;

        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new();
        private readonly BatteryFeature _batteryFeature = new();
        private readonly FlipToStartFeature _flipToStartFeature = new();
        private readonly FnLockFeature _fnLockFeature = new();
        private readonly HybridModeFeature _hybridModeFeature = new();
        private readonly OverDriveFeature _overDriveFeature = new();
        private readonly PowerModeFeature _powerModeFeature = new();
        private readonly TouchpadLockFeature _touchpadLockFeature = new();

        private readonly PowerModeListener _powerModeListener = new();
        private readonly GPUManager _gpuManager = new();

        public MainWindow()
        {
            InitializeComponent();

            ResizeMode = Settings.Instance.MinimizeOnClose ? ResizeMode.NoResize : ResizeMode.CanMinimize;

            StateChanged += mainWindow_StateChanged;
            IsVisibleChanged += mainWindow_IsVisibleChanged;
            Closing += mainWindow_Closing;

            _powerModeListener.Changed += powerModeListener_Changed;
            _gpuManager.WillRefresh += gpuManager_WillRefresh;
            _gpuManager.Refreshed += gpuManager_Refreshed;

            try
            {
                var vantageEnabled = Vantage.IsEnabled;
                enableVantageMenuItem.IsChecked = vantageEnabled;
                disableVantageMenuItem.IsChecked = !vantageEnabled;
            }
            catch (VantageServiceNotFoundException)
            {
                vantageMenuItem.IsEnabled = false;
            }

            autorunMenuItem.IsChecked = Autorun.IsEnabled;
            minimizeOnCloseMenuItem.IsChecked = Settings.Instance.MinimizeOnClose;

            _alwaysOnUsbButtons = new[] { radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways };
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            _hybridModeButtons = new[] { radioHybridOn, radioHybridOff };
            _powerModeButtons = new[] { radioQuiet, radioBalance, radioPerformance };


            elpsDiscreteGPUStatusActive.Visibility = Visibility.Collapsed;
            elpsDiscreteGPUStatusInactive.Visibility = Visibility.Collapsed;
            updateIndicator.Visibility = Visibility.Collapsed;

            _powerModeListener.Start();

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
            Task.Run(Updates.Check)
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
                    break;
                case WindowState.Normal:
                    BringToForeground();
                    break;
            }
        }

        private void mainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                _gpuManager.Start();
            else
                _gpuManager.Stop();
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Instance.MinimizeOnClose)
            {
                WindowState = WindowState.Minimized;
                e.Cancel = true;
            }
            else
                _powerModeListener.Stop();
        }

        private void gpuManager_WillRefresh(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnDeactivateDiscreteGPU.IsEnabled = false;
            });
        }

        private void gpuManager_Refreshed(object sender, GPUManager.RefreshedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Status == GPUManager.Status.Unknown || e.Status == GPUManager.Status.NVIDIAGPUNotFound)
                {
                    lblDiscreteGPUStatus.Content = "-";
                    lblDiscreteGPUStatus.ToolTip = null;
                    elpsDiscreteGPUStatusActive.Visibility = Visibility.Collapsed;
                    elpsDiscreteGPUStatusInactive.Visibility = Visibility.Collapsed;
                }
                else if (e.IsActive)
                {
                    var status = "Active";
                    if (e.ProcessCount > 0)
                        status += $" ({e.ProcessCount} app{(e.ProcessCount > 1 ? "s" : "")})";
                    lblDiscreteGPUStatus.Content = status;
                    lblDiscreteGPUStatus.ToolTip = e.ProcessCount < 1 ? null : ("Processes:\n" + string.Join("\n", e.ProcessNames));
                    elpsDiscreteGPUStatusActive.Visibility = Visibility.Visible;
                    elpsDiscreteGPUStatusInactive.Visibility = Visibility.Collapsed;
                }
                else
                {
                    lblDiscreteGPUStatus.Content = "Inactive";
                    lblDiscreteGPUStatus.ToolTip = null;
                    elpsDiscreteGPUStatusActive.Visibility = Visibility.Collapsed;
                    elpsDiscreteGPUStatusInactive.Visibility = Visibility.Visible;
                }

                btnDeactivateDiscreteGPU.IsEnabled = e.CanBeDeactivated;
                btnDeactivateDiscreteGPU.ToolTip = e.Status switch
                {
                    GPUManager.Status.NVIDIAGPUNotFound => "Discrete nVidia GPU not found. AMD GPUs are not supported.",
                    GPUManager.Status.MonitorsConnected => "A monitor is connected to nVidia GPU.",
                    GPUManager.Status.DeactivatePossible => "nVidia GPU can be disabled.\n\nRemember, that some programs might crash if you do it.",
                    _ => null,
                };
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

            Power.Restart();
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

        private void btnDeactivateDiscreteGPU_Click(object sender, RoutedEventArgs e) => _gpuManager.DeactivateGPU();

        private void notifyIcon_Open(object sender, RoutedEventArgs e) => BringToForeground();

        private void notifyIcon_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e) => new AboutWindow { Owner = this }.ShowDialog();

        private void settingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow { Owner = this };
            window.Closed += (_, _) => Refresh();
            window.Show();
        }

        private void autorunMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Autorun.IsEnabled)
                Autorun.Disable();
            else
            {
                var result = MessageBox.Show("If you move Lenovo Legion Toolkit to a different location after enabling this option, please disable and enable this option to keep it starting automatically.",
                    "Run on startup",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information);
                if (result == MessageBoxResult.OK)
                    Autorun.Enable();
            }

            autorunMenuItem.IsChecked = Autorun.IsEnabled;
        }

        private void minimizeOnCloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settings = Settings.Instance;
            var minimizeOnClose = !settings.MinimizeOnClose;
            settings.MinimizeOnClose = minimizeOnClose;
            settings.Synchronize();

            minimizeOnCloseMenuItem.IsChecked = minimizeOnClose;

            ResizeMode = minimizeOnClose ? ResizeMode.NoResize : ResizeMode.CanMinimize;
        }

        private void enableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Enable();

            var vantageEnabled = Vantage.IsEnabled;
            enableVantageMenuItem.IsChecked = vantageEnabled;
            disableVantageMenuItem.IsChecked = !vantageEnabled;

            var result = MessageBox.Show("It is recommended to restart Windows after enabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Power.Restart();
        }

        private void disableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Disable();

            var vantageEnabled = Vantage.IsEnabled;
            enableVantageMenuItem.IsChecked = vantageEnabled;
            disableVantageMenuItem.IsChecked = !vantageEnabled;

            var result = MessageBox.Show("It is recommended to restart Windows after disabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Power.Restart();
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}