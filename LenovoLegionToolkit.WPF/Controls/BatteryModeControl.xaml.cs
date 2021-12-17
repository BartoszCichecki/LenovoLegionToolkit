using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class BatteryModeControl
    {
        private readonly BatteryFeature _feature = Container.Resolve<BatteryFeature>();

        public BatteryModeControl()
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
            if (_comboBox.SelectedItem == null)
                return;

            var state = (BatteryState)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }

        private void Refresh()
        {
            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            try
            {
                _comboBox.Items.AddEnumValues<BatteryState>();
                _comboBox.SelectedItem = _feature.GetState();
                Visibility = Visibility.Visible;
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
