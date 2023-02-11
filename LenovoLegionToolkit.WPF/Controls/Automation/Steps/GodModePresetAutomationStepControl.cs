using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps;

public class GodModePresetAutomationStepControl : AbstractAutomationStepControl<GodModePresetAutomationStep>
{
    private readonly ComboBox _comboBox = new()
    {
        MinWidth = 150,
        Visibility = Visibility.Hidden,
        Margin = new(8, 0, 0, 0)
    };

    public GodModePresetAutomationStepControl(GodModePresetAutomationStep step) : base(step)
    {
        Icon = SymbolRegular.Gauge24;
        Title = "Custom Mode preset";
        Subtitle = "Activate Custom Mode preset.";
    }

    public override IAutomationStep CreateAutomationStep()
    {
        var presetId = Guid.Empty;

        if (_comboBox.TryGetSelectedItem(out KeyValuePair<Guid, GodModePreset> value))
            presetId = value.Key;

        return new GodModePresetAutomationStep(presetId);
    }

    protected override UIElement GetCustomControl()
    {
        _comboBox.SelectionChanged += ComboBox_SelectionChanged;
        return _comboBox;
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => RaiseChanged();

    protected override async Task RefreshAsync()
    {
        var state = await AutomationStep.GetStateAsync();
        var presets = state.Presets;
        var selectedPreset = presets.FirstOrDefault(kv => kv.Key == AutomationStep.PresetId);

        _comboBox.SetItems(presets, selectedPreset, kv => kv.Value.Name);
        _comboBox.IsEnabled = presets.Any();
    }

    protected override void OnFinishedLoading() => _comboBox.Visibility = Visibility.Visible;
}