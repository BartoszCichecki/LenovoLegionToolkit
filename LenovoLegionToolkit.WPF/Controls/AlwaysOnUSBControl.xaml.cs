using System.Threading.Tasks;
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

            var state = (AlwaysOnUsbState)_comboBox.SelectedItem;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                _comboBox.Items.Clear();
                _comboBox.Items.AddEnumValues<AlwaysOnUsbState>();
                _comboBox.SelectedItem = await _feature.GetStateAsync();
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
