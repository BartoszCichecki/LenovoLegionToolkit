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
        public static string GetPowerPlanGuid(this PowerModeState state)
        {
            return state switch
            {
                PowerModeState.Quiet => "16edbccd-dee9-4ec4-ace5-2f0b5f2a8975",
                PowerModeState.Balance => "85d583c5-cf2e-4197-80fd-3789a227a72c",
                PowerModeState.Performance => "52521609-efc9-4268-b9ba-67dea73f18b2",
                _ => throw new InvalidOperationException("Unknown state."),
            };
        }
    }

    public class PowerModeFeature : AbstractWmiFeature<PowerModeState>
    {
        public PowerModeFeature() : base("SmartFanMode", 1) { }
    }
}