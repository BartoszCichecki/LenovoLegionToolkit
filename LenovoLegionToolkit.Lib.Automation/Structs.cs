using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation
{
    public struct Delay : IDisplayName
    {
        public int DelaySeconds { get; }

        [JsonConstructor]
        public Delay(int delaySeconds) => DelaySeconds = delaySeconds;

        public string DisplayName
        {
            get
            {
                if (DelaySeconds == 1)
                    return "1 second";
                return $"{DelaySeconds} seconds";
            }
        }
    }

}
