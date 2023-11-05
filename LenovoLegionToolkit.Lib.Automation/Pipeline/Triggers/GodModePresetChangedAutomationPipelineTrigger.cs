﻿using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Resources;
using LenovoLegionToolkit.Lib.Controllers.GodMode;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;

public class GodModePresetChangedAutomationPipelineTrigger : IGodModePresetChangedAutomationPipelineTrigger
{
    [JsonIgnore]
    public string DisplayName => Resource.GodModePresetChangedAutomationPipelineTrigger_DisplayName;

    public Guid PresetId { get; }

    [JsonConstructor]
    public GodModePresetChangedAutomationPipelineTrigger(Guid presetId)
    {
        PresetId = presetId;
    }

    public Task<bool> IsMatchingEvent(IAutomationEvent automationEvent)
    {
        if (automationEvent is not CustomModePresetAutomationEvent e)
            return Task.FromResult(false);

        return Task.FromResult(e.Id == PresetId);
    }

    public async Task<bool> IsMatchingState()
    {
        var controller = IoCContainer.Resolve<GodModeController>();
        return PresetId == await controller.GetActivePresetIdAsync().ConfigureAwait(false);
    }

    public void UpdateEnvironment(ref AutomationEnvironment environment) { /* Ignored */ }

    public IAutomationPipelineTrigger DeepCopy() => new GodModePresetChangedAutomationPipelineTrigger(PresetId);

    public IGodModePresetChangedAutomationPipelineTrigger DeepCopy(Guid presetId) => new GodModePresetChangedAutomationPipelineTrigger(presetId);

    public override bool Equals(object? obj)
    {
        return obj is GodModePresetChangedAutomationPipelineTrigger t && PresetId == t.PresetId;
    }

    public override int GetHashCode() => HashCode.Combine(PresetId);

    public override string ToString() => $"{nameof(PresetId)}: {PresetId}";
}
