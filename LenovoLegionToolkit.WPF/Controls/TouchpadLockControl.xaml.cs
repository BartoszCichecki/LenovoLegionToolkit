using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class TouchpadLockControl
    {
        private readonly TouchpadLockFeature _feature = Container.Resolve<TouchpadLockFeature>();

        public IAsyncCommand Command { get; }

        public TouchpadLockControl()
        {
            InitializeComponent();

            DataContext = this;
            Command = new AsyncCommand(ActionAsync);
        }

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        }

        private async Task ActionAsync()
        {
            var state = _toggleButton.IsChecked.Value ? TouchpadLockState.On : TouchpadLockState.Off;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                Command.Enabled = false;

                _toggleButton.IsChecked = await _feature.GetStateAsync() == TouchpadLockState.On;
                Visibility = Visibility.Visible;

                Command.Enabled = true;
            }
            catch
            {
                _toggleButton.IsChecked = false;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
