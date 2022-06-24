using System;

namespace LenovoLegionToolkit.Lib.System
{

    public class FnKeys : SoftwareDisabler
    {
        protected override string[] ScheduledTasksPaths => Array.Empty<string>();

        protected override string[] ServiceNames => new[]
        {
            "LenovoFnAndFunctionKeys",
        };
    }
}
