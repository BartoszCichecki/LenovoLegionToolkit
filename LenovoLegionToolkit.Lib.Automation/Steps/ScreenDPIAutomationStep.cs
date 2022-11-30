using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using Newtonsoft.Json;
using WindowsDisplayAPI;
using WindowsDisplayAPI.Native.DisplayConfig;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public class ScreenDPIAutomationStep : IAutomationStep<ScreenDPI>
    {
        public ScreenDPI State { get; }

        [JsonConstructor]
        public ScreenDPIAutomationStep(ScreenDPI state) => State = state;

        public Task<bool> IsSupportedAsync() => Task.FromResult(true);

        public Task<ScreenDPI[]> GetAllStatesAsync() => Task.FromResult(new ScreenDPI[] {
            new(100),
            new(125),
            new(150),
            new(175),
            new(200),
        });

        public IAutomationStep DeepCopy() => new ScreenDPIAutomationStep(State);

        public Task RunAsync() => ResetHWMonitorDPI();


        private async Task ResetHWMonitorDPI()
        {
            foreach (var item in Display.GetDisplays())
            {
                var name = item.GetTargetDeviceName();
                //TODO it should be Equals empty (internal screen)
                if (!name.StartsWith("HW", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                var dpiInfo = item.GetDisplaScaleInfo();

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"hw screen name: {name}, current dpi: {dpiInfo.current}");

                if (dpiInfo.current != State.DPI)
                {
                    item.ToPathDisplaySource().CurrentDPIScale = (DisplayConfigSourceDPIScale)(uint)State.DPI;
                    Log.Instance.Trace($"set screen dpi: {State.DisplayName}");
                    MessagingCenter.Publish(new Notification(NotificationType.ScreenDPISet, NotificationDuration.Long, name));
                }

            }
        }

    }
}
