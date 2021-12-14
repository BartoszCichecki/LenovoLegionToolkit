using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    /// <summary>
    /// Interaction logic for OverDriveControl.xaml
    /// </summary>
    public partial class OverDriveControl : UserControl
    {
        private readonly OverDriveFeature _feature = Container.Resolve<OverDriveFeature>();

        public OverDriveControl()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            try
            {
                _toggleButton.IsChecked = _feature.GetState() == OverDriveState.On;
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
            var state = _toggleButton.IsChecked.Value ? OverDriveState.On : OverDriveState.Off;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
