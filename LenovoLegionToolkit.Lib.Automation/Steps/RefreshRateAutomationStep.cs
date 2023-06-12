using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using WindowsDisplayAPI.Exceptions;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class RefreshRateAutomationStep : AbstractFeatureAutomationStep<RefreshRate>
{
    [JsonConstructor]
    public RefreshRateAutomationStep(RefreshRate state) : base(state) { }

    public override Task RunAsync()
    {
        return RetryHelper.RetryAsync(base.RunAsync,
            5,
            TimeSpan.FromSeconds(1),
            ex => ex is ModeChangeException,
            nameof(RefreshRateAutomationStep));
    }

    public override IAutomationStep DeepCopy() => new RefreshRateAutomationStep(State);
}
