using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class DisplayBrightnessAutomationStep(int brightness)
    : IAutomationStep
{
    private readonly DisplayBrightnessController _controller = IoCContainer.Resolve<DisplayBrightnessController>();
    public int Brightness { get; } = brightness;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        return _controller.SetBrightnessAsync(Brightness);
    }

    IAutomationStep IAutomationStep.DeepCopy() => new DisplayBrightnessAutomationStep(Brightness);
}
