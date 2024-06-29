using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class SpectrumKeyboardBacklightImportProfileAutomationStep(string? path)
    : IAutomationStep
{
    private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();

    public string? Path { get; } = path;

    public Task<bool> IsSupportedAsync() => _controller.IsSupportedAsync();

    public async Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        if (Path is null || !await _controller.IsSupportedAsync().ConfigureAwait(false))
            return;

        var profile = await _controller.GetProfileAsync().ConfigureAwait(false);
        await _controller.ImportProfileDescription(profile, Path).ConfigureAwait(false);

        MessagingCenter.Publish(new SpectrumBacklightChangedMessage());
    }

    IAutomationStep IAutomationStep.DeepCopy() => new SpectrumKeyboardBacklightImportProfileAutomationStep(Path);
}
