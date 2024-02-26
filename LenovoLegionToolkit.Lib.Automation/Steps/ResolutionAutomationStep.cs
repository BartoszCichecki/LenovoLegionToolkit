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

    public override Task RunAsync(AutomationContext context, AutomationEnvironment environment)
    {
        return RetryHelper.RetryAsync(() => base.RunAsync(context, environment),
            5,
            TimeSpan.FromSeconds(1),
            ex => ex is ModeChangeException,
            nameof(ResolutionAutomationStep));
    }

    public override IAutomationStep DeepCopy() => new ResolutionAutomationStep(State);
}
