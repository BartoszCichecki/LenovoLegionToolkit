using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Dialogs;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class HybridModeControl
    {
        private readonly HybridModeFeature _feature = Container.Resolve<HybridModeFeature>();

        public HybridModeControl()
        {
            InitializeComponent();
        }

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (await DialogService.ShowDialogAsync("Restart required",
                "Changing Hybrid Mode requires restart. Do you want to restart now?"))
            {
                var state = _toggleButton.IsChecked.Value ? HybridModeState.On : HybridModeState.Off;
                if (state != await _feature.GetStateAsync())
                    await _feature.SetStateAsync(state);
            }

            await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            try
            {
                _toggleButton.IsChecked = await _feature.GetStateAsync() == HybridModeState.On;
                Visibility = Visibility.Visible;
            }
            catch
            {
                _toggleButton.IsChecked = false;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
