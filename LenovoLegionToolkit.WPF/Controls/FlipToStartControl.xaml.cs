using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    /// <summary>
    /// Interaction logic for FlipToStartControl.xaml
    /// </summary>
    public partial class FlipToStartControl : UserControl
    {
        private readonly FlipToStartFeature _feature = Container.Resolve<FlipToStartFeature>();

        public FlipToStartControl()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            try
            {
                _toggleButton.IsChecked = _feature.GetState() == FlipToStartState.On;
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
            var state = _toggleButton.IsChecked.Value ? FlipToStartState.On : FlipToStartState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
