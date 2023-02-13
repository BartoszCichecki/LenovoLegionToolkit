using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class GodModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly GodModeController _controller = IoCContainer.Resolve<GodModeController>();

    private GodModeState? _state;
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
            _buttonsStackPanel.Visibility = Visibility.Hidden;

            var loadingTask = Task.Delay(500);

            _state = await _controller.GetStateAsync();

            if (!_state.HasValue)
                throw new InvalidOperationException("State is null.");

            SetState(_state.Value);

            await loadingTask;

            _buttonsStackPanel.Visibility = Visibility.Visible;
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

    private async Task<bool> ApplyAsync()
    {
        try
        {
            if (!_state.HasValue)
                throw new InvalidOperationException("State is null.");

            var activePresetId = _state.Value.ActivePresetId;
            var presets = _state.Value.Presets;
            var preset = presets[activePresetId];

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

            var newPreset = new GodModePreset
            {
                Name = preset.Name,
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

            var newPresets = new Dictionary<Guid, GodModePreset>(presets)
            {
                [activePresetId] = newPreset
            };

            var newState = new GodModeState
            {
                ActivePresetId = activePresetId,
                Presets = newPresets.AsReadOnlyDictionary(),
            };

            if (await _powerModeFeature.GetStateAsync() != PowerModeState.GodMode)
                await _powerModeFeature.SetStateAsync(PowerModeState.GodMode);

            await _controller.SetStateAsync(newState);
            await _controller.ApplyStateAsync();

            return true;
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't apply settings", ex);

            await _snackBar.ShowAsync(Resource.GodModeSettingsWindow_Error_Apply_Title, ex.Message);

            return false;
        }
    }

    private void SetState(GodModeState state)
    {
        var activePresetId = state.ActivePresetId;
        var preset = state.Presets[activePresetId];

        _presetsComboBox.SetItems(state.Presets.OrderBy(kv => kv.Value.Name), new(activePresetId, preset), kv => kv.Value.Name);

        _addPresetsButton.IsEnabled = state.Presets.Count < 5;
        _deletePresetsButton.IsEnabled = state.Presets.Count > 1;

        var maxValueOffset = preset.MaxValueOffset;

        SetSliderValues(_cpuLongTermPowerLimitCardControl, _cpuLongTermPowerLimitSlider, preset.CPULongTermPowerLimit,
            maxValueOffset);
        SetSliderValues(_cpuShortTermPowerLimitCardControl, _cpuShortTermPowerLimitSlider, preset.CPUShortTermPowerLimit,
            maxValueOffset);
        SetSliderValues(_cpuCrossLoadingLimitCardControl, _cpuCrossLoadingLimitSlider, preset.CPUCrossLoadingPowerLimit,
            maxValueOffset);
        SetSliderValues(_cpuTemperatureLimitCardControl, _cpuTemperatureLimitSlider, preset.CPUTemperatureLimit);
        SetSliderValues(_gpuPowerBoostCardControl, _gpuPowerBoostSlider, preset.GPUPowerBoost, maxValueOffset);
        SetSliderValues(_gpuConfigurableTGPCardControl, _gpuConfigurableTGPSlider, preset.GPUConfigurableTGP,
            maxValueOffset);
        SetSliderValues(_gpuTemperatureLimitCardControl, _gpuTemperatureLimitSlider, preset.GPUTemperatureLimit);

        var fanTableInfo = preset.FanTableInfo;
        if (fanTableInfo.HasValue)
            _fanCurveControl.SetFanTableInfo(fanTableInfo.Value);
        else
            _fanCurveCardControl.Visibility = Visibility.Collapsed;

        _fanCurveCardControl.IsEnabled = !preset.FanFullSpeed;
        _fanFullSpeedToggle.IsChecked = preset.FanFullSpeed;

        _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";
    }

    private void PresetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_state.HasValue)
            return;

        if (!_presetsComboBox.TryGetSelectedItem<KeyValuePair<Guid, GodModePreset>>(out var item))
            return;

        if (_state.Value.ActivePresetId == item.Key)
            return;

        _state = _state.Value with { ActivePresetId = item.Key };
        SetState(_state.Value);
    }

    private async void EditPresetsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_state.HasValue)
            return;

        var activePresetId = _state.Value.ActivePresetId;
        var presets = _state.Value.Presets;
        var preset = presets[activePresetId];

        var result = await MessageBoxHelper.ShowInputAsync(this, Resource.GodModeSettingsWindow_EditPreset_Title, Resource.GodModeSettingsWindow_EditPreset_Message, preset.Name);
        if (string.IsNullOrEmpty(result))
            return;

        var newPresets = new Dictionary<Guid, GodModePreset>(presets)
        {
            [activePresetId] = preset with { Name = result }
        };
        _state = _state.Value with { Presets = newPresets.AsReadOnlyDictionary() };
        SetState(_state.Value);
    }

    private void DeletePresetsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_state.HasValue)
            return;

        if (_state.Value.Presets.Count <= 1)
            return;

        var activePresetId = _state.Value.ActivePresetId;
        var presets = _state.Value.Presets;

        var newPresets = new Dictionary<Guid, GodModePreset>(presets);
        newPresets.Remove(activePresetId);
        var newActivePresetId = newPresets.OrderBy(kv => kv.Value.Name)
            .Select(kv => kv.Key)
            .First();

        _state = new()
        {
            ActivePresetId = newActivePresetId,
            Presets = newPresets.AsReadOnlyDictionary()
        };
        SetState(_state.Value);
    }

    private async void AddPresetsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_state.HasValue)
            return;

        if (_state.Value.Presets.Count >= 5)
            return;

        var result = await MessageBoxHelper.ShowInputAsync(this, Resource.GodModeSettingsWindow_EditPreset_Title, Resource.GodModeSettingsWindow_EditPreset_Message);
        if (string.IsNullOrEmpty(result))
            return;

        var activePresetId = _state.Value.ActivePresetId;
        var presets = _state.Value.Presets;
        var preset = presets[activePresetId];

        var newActivePresetId = Guid.NewGuid();
        var newPreset = preset with { Name = result };
        var newPresets = new Dictionary<Guid, GodModePreset>(presets)
        {
            [newActivePresetId] = newPreset
        };

        _state = new()
        {
            ActivePresetId = newActivePresetId,
            Presets = newPresets.AsReadOnlyDictionary()
        };
        SetState(_state.Value);
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
        var state = await _controller.GetStateAsync();
        var preset = state.Presets[state.ActivePresetId];
        var data = preset.FanTableInfo?.Data;

        if (data is null)
            return;

        var defaultFanTableInfo = new FanTableInfo(data, FanTable.Default);
        _fanCurveControl.SetFanTableInfo(defaultFanTableInfo);
    }

    private async void SaveAndCloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (await ApplyAsync())
            Close();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
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