using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    /// <summary>
    /// Interaction logic for HybridModeControl.xaml
    /// </summary>
    public partial class HybridModeControl : UserControl
    {
        private readonly HybridModeFeature _feature = Container.Resolve<HybridModeFeature>();

        public HybridModeControl()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
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

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var state = _toggleButton.IsChecked.Value ? HybridModeState.On : HybridModeState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
