using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

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
            var mi = Windows.GetMachineInformation();
            if (Compatibility.IsCompatible(mi))
                return;

            MessageBox.Show($"This application is not compatible with {mi.Vendor} {mi.Model}.", Title);
            Environment.Exit(0);
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

        private void radioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (PowerModeState)Array.IndexOf(_powerModeButtons, sender);
            if (_powerModeFeature.GetState() == state)
                return;
            _powerModeFeature.SetState(state);

            Windows.SetPowerPlan(state.GetPowerPlanGuid());
        }

        private void hybridMode_Checked(object sender, RoutedEventArgs e)
        {
            var state = (HybridModeState)Array.IndexOf(_hybridModeButtons, sender);
            if (_hybridModeFeature.GetState() == state)
                return;
            _hybridModeFeature.SetState(state);

            var result = MessageBox.Show("Changing Hybrid Mode requires restart. Do you want to restart now?", "Restart required", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                Refresh();
                return;
            }

            Windows.Restart();
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

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e) => new AboutWindow { Owner = this }.ShowDialog();

        private void EnableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Enable();

            var result = MessageBox.Show("It is recommended to restart Windows after enabling Lenovo Vantage. Do you want to restart now?", "Restart recommended", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            Windows.Restart();
        }

        private void DisableVantageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Vantage.Disable();

            var result = MessageBox.Show("It is recommended to restart Windows after disabling Lenovo Vantage. Do you want to restart now?", "Restart recommended", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            Windows.Restart();
        }
    }
}