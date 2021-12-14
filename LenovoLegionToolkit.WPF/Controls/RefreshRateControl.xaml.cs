using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using Microsoft.Win32;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class RefreshRateControl
    {
        private readonly RefreshRateFeature _feature = Container.Resolve<RefreshRateFeature>();

        public RefreshRateControl()
        {
            InitializeComponent();

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            if (!IsVisible)
                return;

            Refresh();
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

            var state = (RefreshRate)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }

        private void Refresh()
        {
            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            try
            {
                var allStates = _feature.GetAllStates();
                foreach (var item in allStates)
                    _comboBox.Items.Add(item);
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
