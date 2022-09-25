﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers
{
    public class OnStartupAutomationPipelineTrigger : IAutomationPipelineTrigger, IOnStartupAutomationPipelineTrigger, IDisallowDuplicatesAutomationPipelineTrigger
    {
        [JsonIgnore]
        public string DisplayName => "On startup";

        public Task<bool> IsSatisfiedAsync(IAutomationEvent automationEvent)
        {
            return Task.FromResult(automationEvent is StartupAutomationEvent);
        }

        public IAutomationPipelineTrigger DeepCopy() => new OnStartupAutomationPipelineTrigger();

        public override bool Equals(object? obj) => obj is OnStartupAutomationPipelineTrigger;

        public override int GetHashCode() => HashCode.Combine(DisplayName);
    }
}
