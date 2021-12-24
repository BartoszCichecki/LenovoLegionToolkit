using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;
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
            if (!_comboBox.TryGetSelectedItem(out RefreshRate state))
                return;

            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                var items = await _feature.GetAllStatesAsync();
                var selectedItem = await _feature.GetStateAsync();
                _comboBox.SetItems(items, selectedItem);
                Visibility = Visibility.Visible;
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
