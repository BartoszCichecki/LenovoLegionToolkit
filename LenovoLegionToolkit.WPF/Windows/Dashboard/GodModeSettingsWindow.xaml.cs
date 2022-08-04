using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class GodModeSettingsWindow
    {
        private readonly GodModeController _controller = IoCContainer.Resolve<GodModeController>();

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

            var state = await _controller.GetStateAsync();

            _cpuLongTermPowerLimitSlider.Minimum = state.CPULongTermPowerLimit.Min;
            _cpuLongTermPowerLimitSlider.Maximum = state.CPULongTermPowerLimit.Max;
            _cpuLongTermPowerLimitSlider.TickFrequency = state.CPULongTermPowerLimit.Step;
            _cpuLongTermPowerLimitSlider.Value = state.CPULongTermPowerLimit.Value;

            _cpuShortTermPowerLimitSlider.Minimum = state.CPUShortTermPowerLimit.Min;
            _cpuShortTermPowerLimitSlider.Maximum = state.CPUShortTermPowerLimit.Max;
            _cpuShortTermPowerLimitSlider.TickFrequency = state.CPUShortTermPowerLimit.Step;
            _cpuShortTermPowerLimitSlider.Value = state.CPUShortTermPowerLimit.Value;

            _gpuPowerBoostSlider.Minimum = state.GPUPowerBoost.Min;
            _gpuPowerBoostSlider.Maximum = state.GPUPowerBoost.Max;
            _gpuPowerBoostSlider.TickFrequency = state.GPUPowerBoost.Step;
            _gpuPowerBoostSlider.Value = state.GPUPowerBoost.Value;

            _gpuConfigurableTGPSlider.Minimum = state.GPUConfigurableTGP.Min;
            _gpuConfigurableTGPSlider.Maximum = state.GPUConfigurableTGP.Max;
            _gpuConfigurableTGPSlider.TickFrequency = state.GPUConfigurableTGP.Step;
            _gpuConfigurableTGPSlider.Value = state.GPUConfigurableTGP.Value;

            _fanCoolingToggle.IsChecked = state.FanCooling;

            await loadingTask;

            _applyRevertStackPanel.Visibility = Visibility.Visible;
            _loader.IsLoading = false;
        }

        private async Task ApplyAsync()
        {
            var state = await _controller.GetStateAsync();

            var cpuLongTermPowerLimit = state.CPULongTermPowerLimit.WithValue((int)_cpuLongTermPowerLimitSlider.Value);
            var cpuShortTermPowerLimit = state.CPUShortTermPowerLimit.WithValue((int)_cpuShortTermPowerLimitSlider.Value);
            var gpuPowerBoost = state.GPUPowerBoost.WithValue((int)_gpuPowerBoostSlider.Value);
            var gpuConfigurableTGP = state.GPUConfigurableTGP.WithValue((int)_gpuConfigurableTGPSlider.Value);

            var fanCooling = _fanCoolingToggle.IsChecked ?? false;

            var newState = new GodModeState
            {
                CPULongTermPowerLimit = cpuLongTermPowerLimit,
                CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                GPUPowerBoost = gpuPowerBoost,
                GPUConfigurableTGP = gpuConfigurableTGP,
                FanCooling = fanCooling
            };

            await _controller.SetStateAsync(newState).ConfigureAwait(false);
            await _controller.ApplyStateAsync().ConfigureAwait(false);
        }

        private async void ApplyAndCloseButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplyAsync();

            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            await ApplyAsync();
            await RefreshAsync();
        }

        private async void RevertButton_Click(object sender, RoutedEventArgs e) => await RefreshAsync();

        private void CpuLongTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _cpuLongTermPowerLimitValueLabel.Content = $"{e.NewValue}W";

        private void CpuShortTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _cpuShortTermPowerLimitValueLabel.Content = $"{e.NewValue}W";

        private void GpuPowerBoostSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _gpuPowerBoostValueLabel.Content = $"{e.NewValue}W";

        private void GpuConfigurableTGPSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => _gpuConfigurableTGPValueLabel.Content = $"{e.NewValue}W";
    }
}
