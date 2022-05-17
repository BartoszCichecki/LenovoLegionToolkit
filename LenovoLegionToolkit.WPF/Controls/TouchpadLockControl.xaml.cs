using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class TouchpadLockControl
    {
        private readonly TouchpadLockFeature _feature = Container.Resolve<TouchpadLockFeature>();

        public TouchpadLockControl()
        {
            InitializeComponent();
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing || _toggle.IsChecked == null)
                return;

            var state = _toggle.IsChecked.Value ? TouchpadLockState.On : TouchpadLockState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            _toggle.IsChecked = await _feature.GetStateAsync() == TouchpadLockState.On;
        }
    }
}
