using System.Threading.Tasks;
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

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_comboBox.SelectedItem == null)
                return;

            var state = (BatteryState)_comboBox.SelectedItem;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            try
            {
                _comboBox.Items.AddEnumValues<BatteryState>();
                _comboBox.SelectedItem = await _feature.GetStateAsync();
                Visibility = Visibility.Visible;
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
