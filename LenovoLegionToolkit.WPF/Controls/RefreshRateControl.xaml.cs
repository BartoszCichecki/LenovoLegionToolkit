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
            SystemEvents.DisplaySettingsChanged += (sender, e) => Refresh();
        }

        private void Refresh()
        {
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
                _comboBox.Items.Clear();
                _comboBox.SelectedItem = null;
                Visibility = Visibility.Collapsed;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var state = (RefreshRate)_comboBox.SelectedItem;
            if (state != _feature.GetState())
                _feature.SetState(state);
        }
    }
}
