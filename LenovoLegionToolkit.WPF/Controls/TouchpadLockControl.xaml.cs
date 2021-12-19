using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class TouchpadLockControl
    {
        private readonly TouchpadLockFeature _feature = Container.Resolve<TouchpadLockFeature>();

        public TouchpadLockControl()
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
            var state = _toggleButton.IsChecked.Value ? TouchpadLockState.On : TouchpadLockState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                _toggleButton.IsChecked = await _feature.GetStateAsync() == TouchpadLockState.On;
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
