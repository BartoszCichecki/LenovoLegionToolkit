using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.System
{
    public class FnKeys : SoftwareDisabler
    {
        protected override string[] ScheduledTasksPaths => Array.Empty<string>();
        protected override string[] ServiceNames => new[] { "LenovoFnAndFunctionKeys" };
        protected override string[] ProcessNames => new[] { "LenovoUtilityUI", "LenovoUtilityService", "LenovoSmartKey" };

        public override async Task EnableAsync()
        {
            await base.EnableAsync().ConfigureAwait(false);
            Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", true);
        }

        public override async Task DisableAsync()
        {
            await base.DisableAsync().ConfigureAwait(false);
            Registry.SetUWPStartup("LenovoUtility", "LenovoUtilityID", false);
        }
    }
}
