using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class FnLockControl
    {
        private readonly FnLockFeature _feature = Container.Resolve<FnLockFeature>();

        public FnLockControl()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            try
            {
                _toggleButton.IsChecked = _feature.GetState() == FnLockState.On;
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
            var state = _toggleButton.IsChecked.Value ? FnLockState.On : FnLockState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
