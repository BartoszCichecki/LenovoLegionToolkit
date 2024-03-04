using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using WindowsDisplayAPI.Exceptions;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class RefreshRateAutomationStep(RefreshRate state)
    : AbstractFeatureAutomationStep<RefreshRate>(state)
{
    public override Task RunAsync(AutomationContext context, AutomationEnvironment environment)
    {
        return RetryHelper.RetryAsync(() => base.RunAsync(context, environment),
            5,
            TimeSpan.FromSeconds(1),
            ex => ex is ModeChangeException,
            nameof(RefreshRateAutomationStep));
    }

    public override IAutomationStep DeepCopy() => new RefreshRateAutomationStep(State);
}
