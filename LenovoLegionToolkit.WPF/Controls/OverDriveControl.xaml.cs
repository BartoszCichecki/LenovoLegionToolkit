using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class OverDriveControl
    {
        private readonly OverDriveFeature _feature = Container.Resolve<OverDriveFeature>();

        public OverDriveControl()
        {
            InitializeComponent();

            _toggle.OnOffContent();
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing || _toggle.IsChecked == null)
                return;

            var state = _toggle.IsChecked.Value ? OverDriveState.On : OverDriveState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            _toggle.IsChecked = await _feature.GetStateAsync() == OverDriveState.On;
        }
    }
}
