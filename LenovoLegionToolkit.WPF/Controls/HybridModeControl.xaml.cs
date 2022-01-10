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

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing)
                return;

            var result = await DialogService.ShowDialogAsync("Restart required", "Changing Hybrid Mode requires restart. Do you want to restart now?");
            if (result)
            {
                var state = _toggleButton.IsChecked.Value ? HybridModeState.On : HybridModeState.Off;
                if (state != await _feature.GetStateAsync())
                    await _feature.SetStateAsync(state);
            }
            else
            {
                _toggleButton.IsChecked = !_toggleButton.IsChecked;
            }
        }

        protected override async Task OnRefreshAsync()
        {
            _toggleButton.IsChecked = await _feature.GetStateAsync() == HybridModeState.On;
        }
    }
}
