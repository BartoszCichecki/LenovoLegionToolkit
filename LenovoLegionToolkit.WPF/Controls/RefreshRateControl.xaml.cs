using System.Threading.Tasks;
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

        private async void SystemEvents_DisplaySettingsChanged(object sender, System.EventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
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

            var state = (RefreshRate)_comboBox.SelectedItem;
            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            _comboBox.Items.Clear();
            _comboBox.SelectedItem = null;

            try
            {
                _comboBox.Items.AddRange(await _feature.GetAllStatesAsync());
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
