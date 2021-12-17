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

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object sender, PowerModeState e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!IsVisible)
                    return;

                Refresh();
            });
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

            var state = (PowerModeState)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }

        private void Refresh()
        {
            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            try
            {
                _comboBox.Items.AddEnumValues<PowerModeState>();
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
