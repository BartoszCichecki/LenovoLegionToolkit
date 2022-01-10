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

        private async void UserControl_Click(object sender, RoutedEventArgs e)
        {
            if (IsRefreshing)
                return;

            var state = _toggleButton.IsChecked.Value ? TouchpadLockState.On : TouchpadLockState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        protected override async Task OnRefreshAsync()
        {
            _toggleButton.IsChecked = await _feature.GetStateAsync() == TouchpadLockState.On;
        }
    }
}
