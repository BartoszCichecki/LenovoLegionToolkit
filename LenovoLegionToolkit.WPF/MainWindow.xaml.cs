using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Utils;
using Microsoft.Win32;

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
        private readonly RefreshRateFeature _refreshRateFeature = new();

        private readonly PowerModeListener _powerModeListener = new();
        private readonly GPUManager _gpuManager = new();

        public MainWindow()
        {
            InitializeComponent();

            ResizeMode = Settings.Instance.MinimizeOnClose ? ResizeMode.NoResize : ResizeMode.CanMinimize;

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            StateChanged += MainWindow_StateChanged;
            IsVisibleChanged += MainWindow_IsVisibleChanged;
            Closing += MainWindow_Closing;

            _powerModeListener.Changed += PowerModeListener_Changed;
            _gpuManager.WillRefresh += GpuManager_WillRefresh;
            _gpuManager.Refreshed += GpuManager_Refreshed;

            var vantageStatus = Vantage.Status;
            vantageMenuItem.IsEnabled = vantageStatus != VantageStatus.NotFound;
            enableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Enabled;
            disableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Disabled;

            autorunMenuItem.IsChecked = Autorun.IsEnabled;
            minimizeOnCloseMenuItem.IsChecked = Settings.Instance.MinimizeOnClose;

            _alwaysOnUsbButtons = new[] { radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways };
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            _hybridModeButtons = new[] { radioHybridOn, radioHybridOff };
            _powerModeButtons = new[] { radioQuiet, radioBalance, radioPerformance };

            elpsDiscreteGPUStatusActive.Visibility = Visibility.Collapsed;
            elpsDiscreteGPUStatusInactive.Visibility = Visibility.Collapsed;
            updateIndicator.Visibility = Visibility.Collapsed;

            _powerModeFeature.EnsureCorrectPowerPlanIsSet();
            _powerModeListener.Start();

            Refresh();
            CheckUpdates();

            AddTraceMenuItemsIfNecessary();
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
                new FeatureCheck(
                    () =>
                    {
                        var allStates = _refreshRateFeature.GetAllStates();
                        cbRefreshRate.ItemsSource = allStates;
                        cbRefreshRate.SelectedValue = _refreshRateFeature.GetState();
                        cbRefreshRate.IsEnabled = allStates.Length > 1;
                    },
                    () =>
                    {
                        cbRefreshRate.ItemsSource = null;
                        cbRefreshRate.SelectedValue = null;
                        cbRefreshRate.IsEnabled = false;
                    }),
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
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e) => Refresh();

        private void MainWindow_StateChanged(object sender, EventArgs e)
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

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                _gpuManager.Start();
            else
                _gpuManager.Stop();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Instance.MinimizeOnClose)
            {
                WindowState = WindowState.Minimized;
                e.Cancel = true;
            }
            else
                _powerModeListener.Stop();
        }

        private void GpuManager_WillRefresh(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnDeactivateDiscreteGPU.IsEnabled = false;
            });
        }

        private void GpuManager_Refreshed(object sender, GPUManager.RefreshedEventArgs e)
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

        public void PowerModeListener_Changed(object sender, PowerModeState state)
        {
            Dispatcher.Invoke(() => { _powerModeButtons[(int)state].IsChecked = true; });
        }

        private void RadioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (PowerModeState)Array.IndexOf(_powerModeButtons, sender);
            if (_powerModeFeature.GetState() == state)
                return;
            _powerModeFeature.SetState(state);
        }

        private void HybridMode_Checked(object sender, RoutedEventArgs e)
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

        private void RefreshRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;

            var selectedItem = (RefreshRate)e.AddedItems[0];

            if (_refreshRateFeature.GetState() == selectedItem)
                return;

            ((ComboBox)sender).IsEnabled = false;

            _refreshRateFeature.SetState(selectedItem);
        }

        private void Battery_Checked(object sender, RoutedEventArgs e)
        {
            var state = (BatteryState)Array.IndexOf(_batteryButtons, sender);
            if (_batteryFeature.GetState() == state)
                return;
            _batteryFeature.SetState(state);
        }

        private void AlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            var state = (AlwaysOnUsbState)Array.IndexOf(_alwaysOnUsbButtons, sender);
            if (_alwaysOnUsbFeature.GetState() == state)
                return;
            _alwaysOnUsbFeature.SetState(state);
        }

        private void FlipToStart_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFlipToStart.IsChecked.GetValueOrDefault(false)
                ? FlipToStartState.On
                : FlipToStartState.Off;
            if (_flipToStartFeature.GetState() == state)
                return;
            _flipToStartFeature.SetState(state);
        }

        private void OverDrive_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkOverDrive.IsChecked.GetValueOrDefault(false)
                ? OverDriveState.On
                : OverDriveState.Off;
            if (_overDriveFeature.GetState() == state)
                return;
            _overDriveFeature.SetState(state);
        }

        private void FnLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFnLock.IsChecked.GetValueOrDefault(false)
                ? FnLockState.On
                : FnLockState.Off;
            if (_fnLockFeature.GetState() == state)
                return;
            _fnLockFeature.SetState(state);
        }

        private void TouchpadLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkTouchpadLock.IsChecked.GetValueOrDefault(false)
                ? TouchpadLockState.On
                : TouchpadLockState.Off;
            if (_touchpadLockFeature.GetState() == state)
                return;
            _touchpadLockFeature.SetState(state);
        }

        private void DeactivateDiscreteGPU_Click(object sender, RoutedEventArgs e) => _gpuManager.DeactivateGPU();

        private void NotifyIcon_Open(object sender, RoutedEventArgs e) => BringToForeground();

        private void NotifyIcon_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e) => new AboutWindow { Owner = this }.ShowDialog();

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow { Owner = this };
            window.Closed += (_, _) =>
            {
                Refresh();

                _powerModeFeature.EnsureCorrectPowerPlanIsSet();
            };
            window.Show();
        }

        private void AutorunMenuItem_Click(object sender, RoutedEventArgs e)
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

        private void MinimizeOnCloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settings = Settings.Instance;
            var minimizeOnClose = !settings.MinimizeOnClose;
            settings.MinimizeOnClose = minimizeOnClose;
            settings.Synchronize();

            minimizeOnCloseMenuItem.IsChecked = minimizeOnClose;

            ResizeMode = minimizeOnClose ? ResizeMode.NoResize : ResizeMode.CanMinimize;
        }

        private void EnableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Enable();

            var vantageStatus = Vantage.Status;
            vantageMenuItem.IsEnabled = vantageStatus != VantageStatus.NotFound;
            enableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Enabled;
            disableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Disabled;

            var result = MessageBox.Show("It is recommended to restart Windows after enabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Power.Restart();
        }

        private void DisableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Disable();

            var vantageStatus = Vantage.Status;
            vantageMenuItem.IsEnabled = vantageStatus != VantageStatus.NotFound;
            enableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Enabled;
            disableVantageMenuItem.IsChecked = vantageStatus == VantageStatus.Disabled;

            var result = MessageBox.Show("It is recommended to restart Windows after disabling Lenovo Vantage.\n\nDo you want to restart now?",
                "Restart recommended",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            Power.Restart();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void AddTraceMenuItemsIfNecessary()
        {
            if (!Log.Instance.IsTraceEnabled)
                return;

            var position = toolsMenuItem.Items.Count - 2;

            var openLogMenuItem = new MenuItem { Header = "Show Log" };
            openLogMenuItem.Click += (s, e) => Process.Start("explorer", Log.Instance.LogPath);

            var openDataFolderMenuItem = new MenuItem { Header = "Open Data folder" };
            openDataFolderMenuItem.Click += (s, e) =>
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                Process.Start("explorer", Path.Combine(localAppData, "LenovoLegionToolkit"));
            };

            var items = toolsMenuItem.Items;
            items.Insert(position, openDataFolderMenuItem);
            items.Insert(position, openLogMenuItem);
            items.Insert(position, new Separator());
        }
    }
}