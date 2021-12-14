using System;
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

        private void Refresh()
        {
            try
            {
                foreach (var item in Enum.GetValues(typeof(BatteryState)))
                    _comboBox.Items.Add(item);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = (BatteryState)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
