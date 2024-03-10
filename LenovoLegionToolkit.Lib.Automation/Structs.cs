using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation;

[method: JsonConstructor]
public readonly struct Delay(int delaySeconds) : IDisplayName
{
    public int DelaySeconds { get; } = delaySeconds;

    public string DisplayName => string.Format(DelaySeconds == 1 ? Resource.Delay_Second : Resource.Delay_Second_Many, DelaySeconds);
}
