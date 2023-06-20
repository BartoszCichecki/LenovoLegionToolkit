using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using WindowsDisplayAPI.Exceptions;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class ResolutionAutomationStep : AbstractFeatureAutomationStep<Resolution>
{
    [JsonConstructor]
    public ResolutionAutomationStep(Resolution state) : base(state) { }

    public override Task RunAsync(AutomationEnvironment environment)
    {
        return RetryHelper.RetryAsync(() => base.RunAsync(environment),
            5,
            TimeSpan.FromSeconds(1),
            ex => ex is ModeChangeException,
            nameof(ResolutionAutomationStep));
    }

    public override IAutomationStep DeepCopy() => new ResolutionAutomationStep(State);
}
