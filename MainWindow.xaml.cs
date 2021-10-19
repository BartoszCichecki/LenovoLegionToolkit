using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            internal FeatureCheck(Action check, Action disable)
            {
                _check = check;
                _disable = disable;
            }

            internal void Check() => _check();
            internal void Disable() => _disable();
        }

        private static readonly string[] compatibleVersions = new string[] {
            "Legion 5 Pro 16ACH6H",
        };

        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new AlwaysOnUsbFeature();
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();
        private readonly FlipToBootFeature _flipToBootFeature = new FlipToBootFeature();
        private readonly FnLockFeature _fnLockFeature = new FnLockFeature();
        private readonly HybridModeFeature _hybridModeFeature = new HybridModeFeature();
        private readonly OverDriveFeature _overDriveFeature = new OverDriveFeature();
        private readonly PowerModeFeature _powerModeFeature = new PowerModeFeature();
        private readonly TouchpadLockFeature _touchpadLockFeature = new TouchpadLockFeature();
        private readonly VantageController _vantageController = new VantageController();

        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly RadioButton[] _batteryButtons;
        private readonly RadioButton[] _hybridModeButtons;
        private readonly RadioButton[] _powerModeButtons;

        public MainWindow()
        {
            InitializeComponent();

            CheckCompatibility();

            _alwaysOnUsbButtons = new[] { radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways };
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            _hybridModeButtons = new[] { radioHybridOn, radioHybridOff };
            _powerModeButtons = new[] { radioQuiet, radioBalance, radioPerformance };

            Refresh();
        }

        private void CheckCompatibility()
        {
            if (compatibleVersions.Contains(Utils.GetMachineVersion()))
                return;

            MessageBox.Show("This application is not compatible with your machine.");
            Environment.Exit(0);
        }

        private void Refresh()
        {
            var features = new[]
            {
                new FeatureCheck(
                    () => _powerModeButtons[(int) _powerModeFeature.GetState()].IsChecked = true,
                    () => DisableControls(_powerModeButtons)
                ),
                new FeatureCheck(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableControls(_batteryButtons)
                ),
                new FeatureCheck(
                    () => _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true,
                    () => DisableControls(_alwaysOnUsbButtons)
                ),
                new FeatureCheck(
                    () => chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On,
                    () => chkOverDrive.IsEnabled = false
                ),
                new FeatureCheck(
                    () => chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On,
                    () => chkTouchpadLock.IsEnabled = false
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
                    () => chkFlipToBoot.IsChecked = _flipToBootFeature.GetState() == FlipToBootState.On,
                    () => chkFlipToBoot.IsEnabled = false
                ),
            };

            foreach (var feature in features)
            {
                try { feature.Check(); }
                catch { feature.Disable(); }
            }
        }

        private void DisableControls(Control[] buttons)
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void radioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (PowerModeState)Array.IndexOf(_powerModeButtons, sender);
            _powerModeFeature.SetState(state);

            Utils.SetPowerPlan(state.PowerPlanGuid());
        }

        private void hybridMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (HybridModeState)Array.IndexOf(_hybridModeButtons, sender);
            _hybridModeFeature.SetState(state);

            if (state == _hybridModeFeature.GetState())
                return;

            var result = MessageBox.Show("Changing Hybrid Mode requires restart. Do you want to restart now?", "Restart required", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                Refresh();
                return;
            }

            Utils.RestartWindows();
        }

        private void radioBattery_Checked(object sender, RoutedEventArgs e)
        {
            _batteryFeature.SetState((BatteryState)Array.IndexOf(_batteryButtons, sender));
        }

        private void radioAlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            _alwaysOnUsbFeature.SetState((AlwaysOnUsbState)Array.IndexOf(_alwaysOnUsbButtons, sender));
        }

        private void chkFlipToBoot_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFlipToBoot.IsChecked.GetValueOrDefault(false)
                ? FlipToBootState.On
                : FlipToBootState.Off;
            _flipToBootFeature.SetState(state);
        }

        private void chkOverDrive_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkOverDrive.IsChecked.GetValueOrDefault(false)
                ? OverDriveState.On
                : OverDriveState.Off;
            _overDriveFeature.SetState(state);
        }

        private void chkFnLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFnLock.IsChecked.GetValueOrDefault(false)
                ? FnLockState.On
                : FnLockState.Off;
            _fnLockFeature.SetState(state);
        }

        private void chkTouchpadLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkTouchpadLock.IsChecked.GetValueOrDefault(false)
                ? TouchpadLockState.On
                : TouchpadLockState.Off;
            _touchpadLockFeature.SetState(state);
        }

        private void EnableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _vantageController.Enable();

            var result = MessageBox.Show("It is recommended to restart Windows after enabling Lenovo Vantage. Do you want to restart now?", "Restart recommended", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            Utils.RestartWindows();
        }

        private void DisableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _vantageController.Disable();

            var result = MessageBox.Show("It is recommended to restart Windows after disabling Lenovo Vantage. Do you want to restart now?", "Restart recommended", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            Utils.RestartWindows();
        }
    }
}