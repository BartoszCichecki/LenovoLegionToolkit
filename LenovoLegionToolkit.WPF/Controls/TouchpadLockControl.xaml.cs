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

        private void Refresh()
        {
            try
            {
                _toggleButton.IsChecked = _feature.GetState() == TouchpadLockState.On;
                Visibility = Visibility.Visible;
            }
            catch
            {
                _toggleButton.IsChecked = false;
                Visibility = Visibility.Collapsed;
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _toggleButton.IsChecked.Value ? TouchpadLockState.On : TouchpadLockState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
