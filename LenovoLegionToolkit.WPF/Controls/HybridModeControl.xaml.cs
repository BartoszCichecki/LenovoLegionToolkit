using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class HybridModeControl
    {
        private readonly HybridModeFeature _feature = Container.Resolve<HybridModeFeature>();

        public HybridModeControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            Refresh();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _toggleButton.IsChecked.Value ? HybridModeState.On : HybridModeState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }

        private void Refresh()
        {
            try
            {
                _toggleButton.IsChecked = _feature.GetState() == HybridModeState.On;
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
