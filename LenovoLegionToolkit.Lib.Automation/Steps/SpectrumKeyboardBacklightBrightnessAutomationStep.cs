using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class SpectrumKeyboardBacklightBrightnessAutomationStep(int state)
    : IAutomationStep<int>
{
    private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();

    private readonly int[] _allStates = Enumerable.Range(0, 10).ToArray();

    public int State { get; } = state;

    public Task<int[]> GetAllStatesAsync() => Task.FromResult(_allStates);

    public Task<bool> IsSupportedAsync() => _controller.IsSupportedAsync();

    public async Task RunAsync(AutomationContext context, AutomationEnvironment environment)
    {
        if (!await _controller.IsSupportedAsync().ConfigureAwait(false))
            return;

        if (!_allStates.Contains(State))
            throw new InvalidOperationException(nameof(State));

        await _controller.SetBrightnessAsync(State).ConfigureAwait(false);
    }

    public IAutomationStep DeepCopy() => new SpectrumKeyboardBacklightBrightnessAutomationStep(State);
}
