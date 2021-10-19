using System;

namespace LenovoLegionToolkit.Lib.Features
{
    public enum PowerModeState
    {
        Quiet,
        Balance,
        Performance
    }

    public static class PowerModeStateExtensions
    {
        public static string PowerPlanGuid(this PowerModeState state)
        {
            switch (state)
            {
                case PowerModeState.Quiet:
                    return "16edbccd-dee9-4ec4-ace5-2f0b5f2a8975";
                case PowerModeState.Balance:
                    return "85d583c5-cf2e-4197-80fd-3789a227a72c";
                case PowerModeState.Performance:
                    return "52521609-efc9-4268-b9ba-67dea73f18b2";
                default:
                    throw new InvalidOperationException("Unknown state.");
            }
        }
    }

    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        public PowerModeFeature() : base("SmartFanMode", 1)
        {
        }
    }
}