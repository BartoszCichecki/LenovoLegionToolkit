using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class FnLockControl
    {
        private readonly FnLockFeature _feature = Container.Resolve<FnLockFeature>();

        public FnLockControl()
        {
            InitializeComponent();
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing || _toggle.IsChecked == null)
                return;

            var state = _toggle.IsChecked.Value ? FnLockState.On : FnLockState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            _toggle.IsChecked = await _feature.GetStateAsync() == FnLockState.On;
        }
    }
}
