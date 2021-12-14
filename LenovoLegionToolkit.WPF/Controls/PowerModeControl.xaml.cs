using System;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class PowerModeControl
    {
        private readonly PowerModeFeature _feature = Container.Resolve<PowerModeFeature>();
        private readonly PowerModeListener _listener = Container.Resolve<PowerModeListener>();

        public PowerModeControl()
        {
            InitializeComponent();

            _listener.Changed += (s, e) => Dispatcher.Invoke(Refresh);
        }

        private void Refresh()
        {
            try
            {
                foreach (var item in Enum.GetValues(typeof(PowerModeState)))
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
            var state = (PowerModeState)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
