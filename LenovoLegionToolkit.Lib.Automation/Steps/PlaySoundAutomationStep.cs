using System.Media;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

[method: JsonConstructor]
public class PlaySoundAutomationStep(string? path) : IAutomationStep
{
    public string? Path { get; } = path;

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationContext context, AutomationEnvironment environment, CancellationToken token)
    {
        if (Path is null)
            return Task.CompletedTask;

        return Task.Run(() =>
        {
            var player = new SoundPlayer(Path);
            player.PlaySync();
        }, token);
    }

    public IAutomationStep DeepCopy() => new PlaySoundAutomationStep(Path);
}
