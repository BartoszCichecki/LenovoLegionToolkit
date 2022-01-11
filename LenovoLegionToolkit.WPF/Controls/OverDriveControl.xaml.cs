using System.Threading.Tasks;
using System.Windows;
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
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing || _toggleButton.IsChecked == null)
                return;

            var state = _toggleButton.IsChecked.Value ? OverDriveState.On : OverDriveState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            _toggleButton.IsChecked = await _feature.GetStateAsync() == OverDriveState.On;
        }
    }
}
