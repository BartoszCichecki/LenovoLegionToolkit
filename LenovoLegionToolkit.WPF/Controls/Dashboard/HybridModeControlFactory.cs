using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Features.Hybrid;
using LenovoLegionToolkit.Lib.Features.Hybrid.Notify;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Dashboard;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public static class HybridModeControlFactory
{
    public static async Task<AbstractRefreshingControl> GetControlAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync();
        return mi.Properties.SupportsIGPUMode
            ? new ComboBoxHybridModeControl()
            : new ToggleHybridModeControl();
    }

    private class ComboBoxHybridModeControl : AbstractComboBoxFeatureCardControl<HybridModeState>
    {
        private readonly DGPUNotify _dgpuNotify = IoCContainer.Resolve<DGPUNotify>();

        private readonly Button _infoButton = new()
        {
            Icon = SymbolRegular.Info24,
            FontSize = 20,
            Margin = new(8, 0, 0, 0),
        };

        public ComboBoxHybridModeControl()
        {
            Icon = SymbolRegular.LeafOne24;
            Title = Resource.ComboBoxHybridModeControl_Title;
            Subtitle = Resource.ComboBoxHybridModeControl_Message;

            AutomationProperties.SetName(_infoButton, Resource.ComboBoxHybridModeControl_Title);
            AutomationProperties.SetHelpText(_infoButton, Resource.Information);

            _dgpuNotify.Notified += DGPUNotify_Notified;
        }

        protected override FrameworkElement GetAccessory(ComboBox comboBox)
        {
            comboBox.MinWidth = 150;

            _infoButton.Click += InfoButton_Click;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(_infoButton);

            return stackPanel;
        }

        protected override async Task OnStateChangeAsync(ComboBox comboBox, IFeature<HybridModeState> feature, HybridModeState? newValue, HybridModeState? oldValue)
        {
            if (newValue is null || oldValue is null)
                return;

            var reboot = (newValue == HybridModeState.Off || oldValue == HybridModeState.Off) && await MessageBoxHelper.ShowAsync(this,
                    Resource.ComboBoxHybridModeControl_RestartRequired_Title,
                    string.Format(Resource.ComboBoxHybridModeControl_RestartRequired_Message, newValue.GetDisplayName()),
                    Resource.RestartNow,
                    Resource.RestartLater);

            await base.OnStateChangeAsync(comboBox, feature, newValue, oldValue);

            if (reboot)
            {
                await Power.RestartAsync();
                return;
            }

            await RefreshAsync();
        }

        protected override void OnStateChangeException(Exception exception)
        {
            if (exception is IGPUModeChangeException ex1)
            {
                var (title, message) = ex1.IGPUMode switch
                {
                    IGPUModeState.IGPUOnly => (Resource.IGPUModeChangeException_Title_IGPUOnly, Resource.IGPUModeChangeException_Message_IGPUOnly),
                    IGPUModeState.Auto => (Resource.IGPUModeChangeException_Title_Auto, Resource.IGPUModeChangeException_Message_Auto),
                    _ => (Resource.IGPUModeChangeException_Title, Resource.IGPUModeChangeException_Message)
                };

                SnackbarHelper.Show(title, message, SnackbarType.Info);
            }
        }

        protected override TimeSpan AdditionalStateChangeDelay(HybridModeState? oldValue, HybridModeState? newValue)
        {
            if (oldValue == HybridModeState.Off || newValue == HybridModeState.Off)
                return TimeSpan.Zero;

            return TimeSpan.FromSeconds(5);
        }

        private void DGPUNotify_Notified(object? sender, bool e) => Dispatcher.Invoke(() =>
        {
            SnackbarHelper.Show(e ? Resource.DGPU_Connected_Title : Resource.DGPU_Disconnected_Title, type: SnackbarType.Info);
        });

        private async void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var states = await Feature.GetAllStatesAsync();
            var window = new ExtendedHybridModeInfoWindow(states) { Owner = Window.GetWindow(this) };
            window.ShowDialog();
        }
    }

    private class ToggleHybridModeControl : AbstractToggleFeatureCardControl<HybridModeState>
    {
        protected override HybridModeState OnState => HybridModeState.On;

        protected override HybridModeState OffState => HybridModeState.Off;

        public ToggleHybridModeControl()
        {
            Icon = SymbolRegular.LeafOne24;
            Title = Resource.ToggleHybridModeControl_Title;
            Subtitle = Resource.ToggleHybridModeControl_Message;
        }

        protected override async Task OnStateChange(ToggleSwitch toggle, IFeature<HybridModeState> feature)
        {
            await base.OnStateChange(toggle, feature);

            var result = await MessageBoxHelper.ShowAsync(
                this,
                Resource.ToggleHybridModeControl_RestartRequired_Title,
                Resource.ToggleHybridModeControl_RestartRequired_Message,
                Resource.RestartNow,
                Resource.RestartLater);

            if (result)
                await Power.RestartAsync();
        }
    }
}
