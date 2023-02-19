using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class GodModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly GodModeControllerV1 _godModeControllerV1 = IoCContainer.Resolve<GodModeControllerV1>();

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

            _state = await _godModeControllerV1.GetStateAsync();

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

            var newPreset = new GodModePreset
            {
                Name = preset.Name,
                CPULongTermPowerLimit = preset.CPULongTermPowerLimit?.WithValue(_cpuLongTermPowerLimitControl.Value),
                CPUShortTermPowerLimit = preset.CPUShortTermPowerLimit?.WithValue(_cpuShortTermPowerLimitControl.Value),
                CPUPeakPowerLimit = preset.CPUPeakPowerLimit?.WithValue(_cpuPeakPowerLimitControl.Value),
                CPUCrossLoadingPowerLimit = preset.CPUCrossLoadingPowerLimit?.WithValue(_cpuCrossLoadingLimitControl.Value),
                CPUPL1Tau = preset.CPUPL1Tau?.WithValue(_cpuPL1TauControl.Value),
                APUsPPTPowerLimit = preset.APUsPPTPowerLimit?.WithValue(_apuSPPTPowerLimitControl.Value),
                CPUTemperatureLimit = preset.CPUTemperatureLimit?.WithValue(_cpuTemperatureLimitControl.Value),
                GPUPowerBoost = preset.GPUPowerBoost?.WithValue(_gpuPowerBoostControl.Value),
                GPUConfigurableTGP = preset.GPUConfigurableTGP?.WithValue(_gpuConfigurableTGPControl.Value),
                GPUTemperatureLimit = preset.GPUTemperatureLimit?.WithValue(_gpuTemperatureLimitControl.Value),
                GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline = preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline?.WithValue(_gpuTotalProcessingPowerTargetOnAcOffsetFromBaselineControl.Value),
                FanTableInfo = _fanCurveControl.GetFanTableInfo(),
                FanFullSpeed = _fanFullSpeedToggle.IsChecked ?? false,
                MaxValueOffset = (int)_maxValueOffsetNumberBox.Value,
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

            await _godModeControllerV1.SetStateAsync(newState);
            await _godModeControllerV1.ApplyStateAsync();

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

        _cpuLongTermPowerLimitControl.Set(preset.CPULongTermPowerLimit, maxValueOffset);
        _cpuShortTermPowerLimitControl.Set(preset.CPUShortTermPowerLimit, maxValueOffset);
        _cpuPeakPowerLimitControl.Set(preset.CPUPeakPowerLimit, maxValueOffset);
        _cpuCrossLoadingLimitControl.Set(preset.CPUCrossLoadingPowerLimit, maxValueOffset);
        _cpuPL1TauControl.Set(preset.CPUPL1Tau);
        _apuSPPTPowerLimitControl.Set(preset.APUsPPTPowerLimit, maxValueOffset);
        _cpuTemperatureLimitControl.Set(preset.CPUTemperatureLimit);
        _gpuPowerBoostControl.Set(preset.GPUPowerBoost, maxValueOffset);
        _gpuConfigurableTGPControl.Set(preset.GPUConfigurableTGP, maxValueOffset);
        _gpuTemperatureLimitControl.Set(preset.GPUTemperatureLimit);
        _gpuTotalProcessingPowerTargetOnAcOffsetFromBaselineControl.Set(preset.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline);

        var fanTableInfo = preset.FanTableInfo;
        if (fanTableInfo.HasValue)
            _fanCurveControl.SetFanTableInfo(fanTableInfo.Value);
        else
            _fanCurveCardControl.Visibility = Visibility.Collapsed;

        var fanFullSpeed = preset.FanFullSpeed;
        if (fanFullSpeed.HasValue)
        {
            _fanCurveCardControl.IsEnabled = !fanFullSpeed.Value;
            _fanFullSpeedToggle.IsChecked = fanFullSpeed.Value;
        }
        else
        {
            _fanCurveCardControl.IsEnabled = true;
            _fanFullSpeedCardControl.Visibility = Visibility.Collapsed;
        }

        if (maxValueOffset.HasValue)
            _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";
        else
            _maxValueOffsetCardControl.Visibility = Visibility.Collapsed;

        var cpuSectionVisible = new[]
        {
            _cpuLongTermPowerLimitControl,
            _cpuShortTermPowerLimitControl,
            _cpuPeakPowerLimitControl,
            _cpuCrossLoadingLimitControl,
            _cpuPL1TauControl,
            _apuSPPTPowerLimitControl,
            _cpuTemperatureLimitControl
        }.Any(v => v.Visibility == Visibility.Visible);

        var gpuSectionVisible = new[]
        {
            _gpuPowerBoostControl,
            _gpuConfigurableTGPControl,
            _gpuTemperatureLimitControl,
            _gpuTotalProcessingPowerTargetOnAcOffsetFromBaselineControl
        }.Any(v => v.Visibility == Visibility.Visible);

        var fanSectionVisible = new[]
        {
            _fanCurveCardControl,
            _fanFullSpeedCardControl,
        }.Any(v => v.Visibility == Visibility.Visible);

        var advancedSectionVisible = _maxValueOffsetCardControl.Visibility == Visibility.Visible;

        _cpuSectionTitle.Visibility = cpuSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _gpuSectionTitle.Visibility = gpuSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _fanSectionTitle.Visibility = fanSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _advancedSectionTitle.Visibility = advancedSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _advancedSectionMessage.Visibility = advancedSectionVisible ? Visibility.Visible : Visibility.Collapsed;
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

    private async void ResetFanCurve_Click(object sender, RoutedEventArgs e)
    {
        var state = await _godModeControllerV1.GetStateAsync();
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

        if (_cpuLongTermPowerLimitControl.Value > _cpuShortTermPowerLimitControl.Value)
            _cpuShortTermPowerLimitControl.Value = _cpuLongTermPowerLimitControl.Value;
    }

    private void CpuShortTermPowerLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isRefreshing)
            return;

        if (_cpuLongTermPowerLimitControl.Value > _cpuShortTermPowerLimitControl.Value)
            _cpuLongTermPowerLimitControl.Value = _cpuShortTermPowerLimitControl.Value;
    }

    private void FanFullSpeedToggle_Click(object sender, RoutedEventArgs e)
    {
        _fanCurveCardControl.IsEnabled = !(_fanFullSpeedToggle.IsChecked ?? false);
    }
}