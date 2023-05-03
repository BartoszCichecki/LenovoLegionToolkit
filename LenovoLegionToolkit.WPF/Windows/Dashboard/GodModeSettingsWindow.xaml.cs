using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class GodModeSettingsWindow
{
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly GodModeController _godModeController = IoCContainer.Resolve<GodModeController>();

    private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();
    private readonly LegionZone _legionZone = IoCContainer.Resolve<LegionZone>();

    private GodModeState? _state;
    private Dictionary<PowerModeState, GodModeDefaults>? _defaults;
    private bool _isRefreshing;

    public GodModeSettingsWindow() => InitializeComponent();

    private async void GodModeSettingsWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _isRefreshing = true;

        try
        {
            _loader.IsLoading = true;
            _buttonsStackPanel.Visibility = Visibility.Hidden;

            var loadingTask = Task.Delay(TimeSpan.FromMilliseconds(500));

            _vantageRunningWarningInfoBar.Visibility = await _godModeController.NeedsVantageDisabledAsync()
                                                       && await _vantage.GetStatusAsync() == SoftwareStatus.Enabled
                ? Visibility.Visible
                : Visibility.Collapsed;

            _legionZoneRunningWarningInfoBar.Visibility = await _godModeController.NeedsLegionZoneDisabledAsync()
                                                          && await _legionZone.GetStatusAsync() == SoftwareStatus.Enabled
                ? Visibility.Visible
                : Visibility.Collapsed;

            _state = await _godModeController.GetStateAsync();
            _defaults = await _godModeController.GetDefaultsInOtherPowerModesAsync();

            if (_state is null)
                throw new InvalidOperationException($"{nameof(_state)} is null.");

            if (_defaults is null)
                throw new InvalidOperationException($"{nameof(_defaults)} are null.");

            SetState(_state.Value);

            await loadingTask;

            _loadButton.Visibility = _defaults.Any() ? Visibility.Visible : Visibility.Collapsed;
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
                FanTableInfo = preset.FanTableInfo is not null ? _fanCurveControl.GetFanTableInfo() : null,
                FanFullSpeed = preset.FanFullSpeed is not null ? _fanFullSpeedToggle.IsChecked : null,
                MaxValueOffset = preset.MaxValueOffset is not null ? (int)_maxValueOffsetNumberBox.Value : null,
                MinValueOffset = preset.MinValueOffset is not null ? (int)_minValueOffsetNumberBox.Value : null
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

            await _godModeController.SetStateAsync(newState);
            await _godModeController.ApplyStateAsync();

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


        _cpuLongTermPowerLimitControl.Set(preset.CPULongTermPowerLimit);
        _cpuShortTermPowerLimitControl.Set(preset.CPUShortTermPowerLimit);
        _cpuPeakPowerLimitControl.Set(preset.CPUPeakPowerLimit);
        _cpuCrossLoadingLimitControl.Set(preset.CPUCrossLoadingPowerLimit);
        _cpuPL1TauControl.Set(preset.CPUPL1Tau);
        _apuSPPTPowerLimitControl.Set(preset.APUsPPTPowerLimit);
        _cpuTemperatureLimitControl.Set(preset.CPUTemperatureLimit);
        _gpuPowerBoostControl.Set(preset.GPUPowerBoost);
        _gpuConfigurableTGPControl.Set(preset.GPUConfigurableTGP);
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

        var maxValueOffset = preset.MaxValueOffset;
        if (maxValueOffset.HasValue)
            _maxValueOffsetNumberBox.Text = $"{maxValueOffset}";
        else
            _maxValueOffsetCardControl.Visibility = Visibility.Collapsed;

        var minValueOffset = preset.MinValueOffset;
        if (minValueOffset.HasValue)
            _minValueOffsetNumberBox.Text = $"{minValueOffset}";
        else
            _minValueOffsetCardControl.Visibility = Visibility.Collapsed;

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
            _fanFullSpeedCardControl
        }.Any(v => v.Visibility == Visibility.Visible);

        var advancedSectionVisible = new[]
        {
            _maxValueOffsetCardControl,
            _minValueOffsetCardControl
        }.Any(v => v.Visibility == Visibility.Visible);

        _cpuSectionTitle.Visibility = cpuSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _gpuSectionTitle.Visibility = gpuSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _fanSectionTitle.Visibility = fanSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _advancedSectionTitle.Visibility = advancedSectionVisible ? Visibility.Visible : Visibility.Collapsed;
        _advancedSectionMessage.Visibility = advancedSectionVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void SetDefaults(GodModeDefaults defaults)
    {
        if (_cpuLongTermPowerLimitControl.Visibility == Visibility.Visible && defaults.CPULongTermPowerLimit is { } cpuLongTermPowerLimit)
            _cpuLongTermPowerLimitControl.Value = cpuLongTermPowerLimit;

        if (_cpuShortTermPowerLimitControl.Visibility == Visibility.Visible && defaults.CPUShortTermPowerLimit is { } cpuShortTermPowerLimit)
            _cpuShortTermPowerLimitControl.Value = cpuShortTermPowerLimit;

        if (_cpuPeakPowerLimitControl.Visibility == Visibility.Visible && defaults.CPUPeakPowerLimit is { } cpuPeakPowerLimit)
            _cpuPeakPowerLimitControl.Value = cpuPeakPowerLimit;

        if (_cpuCrossLoadingLimitControl.Visibility == Visibility.Visible && defaults.CPUCrossLoadingPowerLimit is { } cpuCrossLoadingPowerLimit)
            _cpuCrossLoadingLimitControl.Value = cpuCrossLoadingPowerLimit;

        if (_cpuPL1TauControl.Visibility == Visibility.Visible && defaults.CPUPL1Tau is { } cpuPL1Tau)
            _cpuPL1TauControl.Value = cpuPL1Tau;

        if (_apuSPPTPowerLimitControl.Visibility == Visibility.Visible && defaults.APUsPPTPowerLimit is { } _apuSPPTPowerLimit)
            _apuSPPTPowerLimitControl.Value = _apuSPPTPowerLimit;

        if (_cpuTemperatureLimitControl.Visibility == Visibility.Visible && defaults.CPUTemperatureLimit is { } cpuTemperatureLimit)
            _cpuTemperatureLimitControl.Value = cpuTemperatureLimit;

        if (_gpuPowerBoostControl.Visibility == Visibility.Visible && defaults.GPUPowerBoost is { } gpuPowerBoost)
            _gpuPowerBoostControl.Value = gpuPowerBoost;

        if (_gpuConfigurableTGPControl.Visibility == Visibility.Visible && defaults.GPUConfigurableTGP is { } gpuConfigurableTgp)
            _gpuConfigurableTGPControl.Value = gpuConfigurableTgp;

        if (_gpuTemperatureLimitControl.Visibility == Visibility.Visible && defaults.GPUTemperatureLimit is { } gpuTemperatureLimit)
            _gpuTemperatureLimitControl.Value = gpuTemperatureLimit;

        if (_gpuTotalProcessingPowerTargetOnAcOffsetFromBaselineControl.Visibility == Visibility.Visible && defaults.GPUTotalProcessingPowerTargetOnAcOffsetFromBaseline is { } gpuTotalProcessingPowerTargetOnAcOffsetFromBaseline)
            _gpuTotalProcessingPowerTargetOnAcOffsetFromBaselineControl.Value = gpuTotalProcessingPowerTargetOnAcOffsetFromBaseline;

        if (_fanCurveCardControl.Visibility == Visibility.Visible && defaults.FanTable is { } fanTable)
        {
            var state = await _godModeController.GetStateAsync();
            var preset = state.Presets[state.ActivePresetId];
            var data = preset.FanTableInfo?.Data;

            if (data is not null)
            {
                var defaultFanTableInfo = new FanTableInfo(data, fanTable);
                _fanCurveControl.SetFanTableInfo(defaultFanTableInfo);
            }
        }

        if (_fanFullSpeedCardControl.Visibility == Visibility.Visible && defaults.FanFullSpeed is { } fanFullSpeed)
            _fanFullSpeedToggle.IsChecked = fanFullSpeed;

        if (_maxValueOffsetCardControl.Visibility == Visibility.Visible)
            _maxValueOffsetNumberBox.Text = $"{0}";

        if (_minValueOffsetCardControl.Visibility == Visibility.Visible)
            _minValueOffsetNumberBox.Text = $"{0}";
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

    private async void DefaultFanCurve_Click(object sender, RoutedEventArgs e)
    {
        var state = await _godModeController.GetStateAsync();
        var preset = state.Presets[state.ActivePresetId];
        var data = preset.FanTableInfo?.Data;

        if (data is null)
            return;

        var defaultFanTableInfo = new FanTableInfo(data, FanTable.Default);
        _fanCurveControl.SetFanTableInfo(defaultFanTableInfo);
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        if (_defaults is null || _defaults.IsEmpty())
        {
            _loadButton.Visibility = Visibility.Collapsed;
            return;
        }

        var menuItems = _defaults
            .OrderBy(d => d.Key)
            .Select(d =>
            {
                var menuItem = new MenuItem { Header = d.Key.GetDisplayName() };
                menuItem.Click += (_, _) => SetDefaults(d.Value);
                return menuItem;
            });

        var contextMenu = new ContextMenu
        {
            PlacementTarget = _loadButton,
            Placement = PlacementMode.Bottom,
        };

        foreach (var menuItem in menuItems)
            contextMenu.Items.Add(menuItem);

        _loadButton.ContextMenu = contextMenu;
        _loadButton.ContextMenu.IsOpen = true;
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