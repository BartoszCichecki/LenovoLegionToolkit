using System;
using System.Linq;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Settings;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class GodModePresetPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<IGodModePresetChangedAutomationPipelineTrigger>
{
    private readonly GodModeSettings _settings = IoCContainer.Resolve<GodModeSettings>();

    private readonly IGodModePresetChangedAutomationPipelineTrigger _trigger;

    public GodModePresetPipelineTriggerTabItemContent(IGodModePresetChangedAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;

        InitializeComponent();
    }

    public IGodModePresetChangedAutomationPipelineTrigger GetTrigger()
    {
        var state = _content.Children
            .OfType<RadioButton>()
            .Where(r => r.IsChecked ?? false)
            .Select(r => (Guid)r.Tag)
            .DefaultIfEmpty(Guid.Empty)
            .FirstOrDefault();
        return _trigger.DeepCopy(state);
    }

    private void GodModePresetPipelineTriggerTabItemContent_Initialized(object? sender, EventArgs e)
    {
        foreach (var (guid, preset) in _settings.Store.Presets)
        {
            var radio = new RadioButton
            {
                Content = preset.Name,
                Tag = guid,
                IsChecked = guid == _trigger.PresetId,
                Margin = new(0, 0, 0, 8)
            };
            _content.Children.Add(radio);
        }
    }
}