using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class GodModeSettingsWindow
    {
        private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
        private readonly GodModeSettings _settings = IoCContainer.Resolve<GodModeSettings>();

        public GodModeSettingsWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += GodModeSettingsWindow_Loaded;
            IsVisibleChanged += GodModeSettingsWindow_IsVisibleChanged;
        }

        private async void GodModeSettingsWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void GodModeSettingsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            _loader.IsLoading = true;
            _applyRevertStackPanel.Visibility = Visibility.Hidden;

            var loadingTask = Task.Delay(500);

            _fanCoolingToggle.IsChecked = _settings.Store.FanCooling ?? false;

            await loadingTask;

            _applyRevertStackPanel.Visibility = Visibility.Visible;
            _loader.IsLoading = false;
        }

        private void Save()
        {
            _settings.Store.FanCooling = _fanCoolingToggle.IsChecked;
            _settings.SynchronizeStore();
        }

        private async void ApplyAndCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Save();

            await _powerModeFeature.SetStateAsync(PowerModeState.GodMode);

            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Save();

            await _powerModeFeature.SetStateAsync(PowerModeState.GodMode);
        }

        private async void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshAsync();
        }
    }
}
