using LenovoLegionToolkit.Lib.Automation.Resources;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation
{
    public struct Delay : IDisplayName
    {
        public int DelaySeconds { get; }

        [JsonConstructor]
        public Delay(int delaySeconds) => DelaySeconds = delaySeconds;

        public string DisplayName => DelaySeconds == 1 ? Resource.Delay_OneSecond : string.Format(Resource.Delay_DisplayName_ManySeconds, DelaySeconds);
    }

}
