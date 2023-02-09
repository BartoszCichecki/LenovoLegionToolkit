using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class GodModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly GodModeController _controller = IoCContainer.Resolve<GodModeController>();

    private bool _isRefreshing;

    public GodModeSettingsWindow() => InitializeComponent();

    private async void GodModeSettingsWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

    private async void GodModeSettingsWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _isRefreshing = true;

        try
        {
            _loader.IsLoading = true;
            _applyRevertStackPanel.Visibility = Visibility.Hidden;

            var loadingTask = Task.Delay(500);

            var (presetId, presets) = await _controller.GetStateAsync();
            var preset = presets.FirstOrDefault(p => p.Id == presetId);

            _presetsComboBox.SetItems(presets, preset, p => p.Name);

            var maxValueOffset = preset.MaxValueOffset;

            SetSliderValues(_cpuLongTermPowerLimitCardControl, _cpuLongTermPowerLimitSlider, preset.CPULongTermPowerLimit, maxValueOffset);
            SetSliderValues(_cpuShortTermPowerLimitCardControl, _cpuShortTermPowerLimitSlider, preset.CPUShortTermPowerLimit, maxValueOffset);
            SetSliderValues(_cpuCrossLoadingLimitCardControl, _cpuCrossLoadingLimitSlider, preset.CPUCrossLoadingPowerLimit, maxValueOffset);
            SetSliderValues(_cpuTemperatureLimitCardControl, _cpuTemperatureLimitSlider, preset.CPUTemperatureLimit);
            SetSliderValues(_gpuPowerBoostCardControl, _gpuPowerBoostSlider, preset.GPUPowerBoost, maxValueOffset);
            SetSliderValues(_gpuConfigurableTGPCardControl, _gpuConfigurableTGPSlider, preset.GPUConfigurableTGP, maxValueOffset);
            SetSliderValues(_gpuTemperatureLimitCardControl, _gpuTemperatureLimitSlider, preset.GPUTemperatureLimit);

            var fanTableInfo = preset.FanTableInfo;
            if (fanTableInfo.HasValue)
                _fanCurveControl.SetFanTableInfo(fanTableInfo.Value);
            else
                _fanCurveCardControl.Visibility = Visibility.Collapsed;

            _fanCurveCardControl.IsEnabled = !preset.FanFullSpeed;
            _fanFullSpeedToggle.IsChecked = preset.FanFullSpeed;

            _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";

            await loadingTask;

            _applyRevertStackPanel.Visibility = Visibility.Visible;
            _loader.IsLoading = false;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't load settings.", ex);

            await _snackBar.ShowAsync(Resource.GodModeSettingsWindow_Error_Load_Title, ex.Message);

            Close();
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private async Task ApplyAsync()
    {
        try
        {
            var (_, presets) = await _controller.GetStateAsync();
            var preset = presets.FirstOrDefault();

            StepperValue? cpuLongTermPowerLimit = null;
            StepperValue? cpuShortTermPowerLimit = null;
            StepperValue? cpuCrossLoadingPowerLimit = null;
            StepperValue? cpuTemperatureLimit = null;
            StepperValue? gpuPowerBoost = null;
            StepperValue? gpuConfigurableTgp = null;
            StepperValue? gpuTemperatureLimit = null;

            if (preset.CPULongTermPowerLimit.HasValue)
                cpuLongTermPowerLimit = preset.CPULongTermPowerLimit.Value.WithValue((int)_cpuLongTermPowerLimitSlider.Value);

            if (preset.CPUShortTermPowerLimit.HasValue)
                cpuShortTermPowerLimit = preset.CPUShortTermPowerLimit.Value.WithValue((int)_cpuShortTermPowerLimitSlider.Value);

            if (preset.CPUCrossLoadingPowerLimit.HasValue)
                cpuCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit.Value.WithValue((int)_cpuCrossLoadingLimitSlider.Value);

            if (preset.CPUTemperatureLimit.HasValue)
                cpuTemperatureLimit = preset.CPUTemperatureLimit.Value.WithValue((int)_cpuTemperatureLimitSlider.Value);

            if (preset.GPUPowerBoost.HasValue)
                gpuPowerBoost = preset.GPUPowerBoost.Value.WithValue((int)_gpuPowerBoostSlider.Value);

            if (preset.GPUConfigurableTGP.HasValue)
                gpuConfigurableTgp = preset.GPUConfigurableTGP.Value.WithValue((int)_gpuConfigurableTGPSlider.Value);

            if (preset.GPUTemperatureLimit.HasValue)
                gpuTemperatureLimit = preset.GPUTemperatureLimit.Value.WithValue((int)_gpuTemperatureLimitSlider.Value);

            var fanTableInfo = _fanCurveControl.GetFanTableInfo();
            var fanFullSpeed = _fanFullSpeedToggle.IsChecked ?? false;

            var maxValueOffset = (int)_maxValueOffsetNumberBox.Value;

            var newState = new GodModeState
            {
                CPULongTermPowerLimit = cpuLongTermPowerLimit,
                CPUShortTermPowerLimit = cpuShortTermPowerLimit,
                CPUCrossLoadingPowerLimit = cpuCrossLoadingPowerLimit,
                CPUTemperatureLimit = cpuTemperatureLimit,
                GPUPowerBoost = gpuPowerBoost,
                GPUConfigurableTGP = gpuConfigurableTgp,
                GPUTemperatureLimit = gpuTemperatureLimit,
                FanTableInfo = fanTableInfo,
                FanFullSpeed = fanFullSpeed,
                MaxValueOffset = maxValueOffset,
            };

            if (await _powerModeFeature.GetStateAsync() != PowerModeState.GodMode)
                await _powerModeFeature.SetStateAsync(PowerModeState.GodMode);

            await _controller.SetStateAsync(newState);
            await _controller.ApplyActiveStateAsync();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't apply settings", ex);

            await _snackBar.ShowAsync(Resource.GodModeSettingsWindow_Error_Apply_Title, ex.Message);
        }
    }

    private void SetSliderValues(CardControl cardControl, Slider slider, StepperValue? stepperValue, int maxValueOffset = 0)
    {
        if (!stepperValue.HasValue)
        {
            cardControl.Visibility = Visibility.Collapsed;
            return;
        }

        slider.Minimum = stepperValue.Value.Min;
        slider.Maximum = stepperValue.Value.Max + maxValueOffset;
        slider.TickFrequency = stepperValue.Value.Step;
        slider.Value = stepperValue.Value.Value;

        if (stepperValue.Value.Min == stepperValue.Value.Max + maxValueOffset)
            slider.IsEnabled = false;
    }

    private async void ResetFanCurve_Click(object sender, RoutedEventArgs e)
    {
        var (_, presets) = await _controller.GetStateAsync();
        var preset = presets.FirstOrDefault();

        var data = preset.FanTableInfo?.Data;

        if (data is null)
            return;

        var defaultFanTableInfo = new FanTableInfo(data, FanTable.Default);
        _fanCurveControl.SetFanTableInfo(defaultFanTableInfo);
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

    private void CpuLongTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isRefreshing)
            return;

        if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
            _cpuShortTermPowerLimitSlider.Value = _cpuLongTermPowerLimitSlider.Value;
    }

    private void CpuShortTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isRefreshing)
            return;

        if (_cpuLongTermPowerLimitSlider.Value > _cpuShortTermPowerLimitSlider.Value)
            _cpuLongTermPowerLimitSlider.Value = _cpuShortTermPowerLimitSlider.Value;
    }

    private void FanFullSpeedToggle_Click(object sender, RoutedEventArgs e)
    {
        _fanCurveCardControl.IsEnabled = !(_fanFullSpeedToggle.IsChecked ?? false);
    }
}