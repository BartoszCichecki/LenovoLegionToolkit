using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Controllers;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class SpectrumKeyboardBacklightImportProfileAutomationStep : IAutomationStep
{
    private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();

    public string? Path { get; }

    [JsonConstructor]
    public SpectrumKeyboardBacklightImportProfileAutomationStep(string? path)
    {
        Path = path;
    }

    public Task<bool> IsSupportedAsync() => _controller.IsSupportedAsync();

    public async Task RunAsync()
    {
        if (Path is null || !await _controller.IsSupportedAsync().ConfigureAwait(false))
            return;

        var profile = await _controller.GetProfileAsync().ConfigureAwait(false);
        await _controller.ImportProfileDescription(profile, Path).ConfigureAwait(false);
    }

    IAutomationStep IAutomationStep.DeepCopy() => new SpectrumKeyboardBacklightImportProfileAutomationStep(Path);
}
