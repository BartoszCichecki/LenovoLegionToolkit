using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation;

public readonly struct Delay : IDisplayName
{
    public int DelaySeconds { get; }

    [JsonConstructor]
    public Delay(int delaySeconds) => DelaySeconds = delaySeconds;

    public string DisplayName => string.Format(DelaySeconds == 1 ? Resource.Delay_Second : Resource.Delay_Second_Many, DelaySeconds);
}