using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class AlwaysOnUSBControl
    {
        private readonly AlwaysOnUsbFeature _feature = Container.Resolve<AlwaysOnUsbFeature>();

        public AlwaysOnUSBControl()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            Refresh();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = (AlwaysOnUsbState)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }

        private void Refresh()
        {
            try
            {
                _comboBox.Items.AddEnumValues<AlwaysOnUsbState>();
                _comboBox.SelectedItem = _feature.GetState();
                Visibility = Visibility.Visible;
            }
            catch
            {
                _comboBox.Items.Clear();
                _comboBox.SelectedItem = null;
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
