using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
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

            _toggle.OnOffContent();
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing || _toggle.IsChecked == null)
                return;

            var result = await DialogService.ShowDialogAsync("Restart required", "Changing Hybrid Mode requires restart. Do you want to restart now?");
            if (result)
            {
                var state = _toggle.IsChecked.Value ? HybridModeState.On : HybridModeState.Off;
                if (state != await _feature.GetStateAsync())
                    await _feature.SetStateAsync(state);
            }
            else
            {
                _toggle.IsChecked = !_toggle.IsChecked;
            }
        }

        protected override async Task OnRefreshAsync()
        {
            _toggle.IsChecked = await _feature.GetStateAsync() == HybridModeState.On;
        }
    }
}
