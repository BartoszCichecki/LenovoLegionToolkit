using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Dashboard;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public class HybridModeControl : ContentControl
    {
        public HybridModeControl()
        {
            Initialized += HybridModeControl_Initialized;
        }

        private async void HybridModeControl_Initialized(object? sender, EventArgs e)
        {
            var mi = await Compatibility.GetMachineInformationAsync();
            if (mi.Properties.SupportsExtendedHybridMode)
                Content = new ComboBoxHybridModeControl();
            else
                Content = new ToggleHybridModeControl();
        }
    }

    public class ComboBoxHybridModeControl : AbstractComboBoxFeatureCardControl<HybridModeState>
    {
        private readonly Button _infoButton = new()
        {
            Icon = SymbolRegular.Info24,
            FontSize = 20,
            Margin = new(8, 0, 0, 0),
        };

        public ComboBoxHybridModeControl()
        {
            Icon = SymbolRegular.LeafOne24;
            Title = "GPU Working Mode";
            Subtitle = "Select GPU operating mode based on your computer's usage and power conditions.\nSwitching modes may require restart.";
        }

        protected override FrameworkElement? GetAccessory(ComboBox comboBox)
        {
            comboBox.Width = 180;

            _infoButton.Click += InfoButton_Click;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(_infoButton);

            return stackPanel;
        }

        protected override async Task OnStateChange(ComboBox comboBox, IFeature<HybridModeState> feature, HybridModeState? newValue, HybridModeState? oldValue)
        {
            if (newValue is null || oldValue is null)
                return;

            await base.OnStateChange(comboBox, feature, newValue, oldValue);

            if (newValue != HybridModeState.Off && oldValue != HybridModeState.Off)
                return;

            var result = await MessageBoxHelper.ShowAsync(
                this,
                "Restart required",
                $"Changing to {newValue.GetDisplayName()} requires restart. Do you want to restart now?",
                "Restart now",
                "I will restart later");

            if (result)
                await Power.RestartAsync();
            else
                await RefreshAsync();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ExtendedHybridModeInfoWindow
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            window.ShowDialog();
        }
    }

    public class ToggleHybridModeControl : AbstractToggleFeatureCardControl<HybridModeState>
    {
        protected override HybridModeState OnState => HybridModeState.On;

        protected override HybridModeState OffState => HybridModeState.Off;

        public ToggleHybridModeControl()
        {
            Icon = SymbolRegular.LeafOne24;
            Title = "Hybrid Mode";
            Subtitle = "Allow switching between integrated and discrete GPU.\nRequires restart.";
        }

        protected override async Task OnStateChange(ToggleSwitch toggle, IFeature<HybridModeState> feature)
        {
            await base.OnStateChange(toggle, feature);

            var result = await MessageBoxHelper.ShowAsync(
                this,
                "Restart required",
                "Changing Hybrid Mode requires restart. Do you want to restart now?",
                "Restart now",
                "I will restart later");

            if (result)
                await Power.RestartAsync();
        }
    }
}
