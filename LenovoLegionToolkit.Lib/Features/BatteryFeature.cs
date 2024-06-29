using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Features;

public class BatteryFeature() : AbstractDriverFeature<BatteryState>(Drivers.GetEnergy, Drivers.IOCTL_ENERGY_BATTERY_CHARGE_MODE)
{
    private const string BATTERY_CHARGE_MODE_HIVE = "HKEY_CURRENT_USER";
    private const string BATTERY_CHARGE_MODE_PATH = "Software\\Lenovo\\VantageService\\AddinData\\IdeaNotebookAddin";
    private const string BATTERY_CHARGE_MODE_KEY = "BatteryChargeMode";
    private const string BATTERY_CHARGE_MODE_NORMAL = "Normal";
    private const string BATTERY_CHARGE_MODE_RAPID_CHARGE = "Quick";
    private const string BATTERY_CHARGE_MODE_CONSERVATION = "Storage";

    protected override uint GetInBufferValue() => 0xFF;

    protected override Task<uint[]> ToInternalAsync(BatteryState state)
    {
        var result = state switch
        {
            BatteryState.Conservation => LastState == BatteryState.RapidCharge ? new uint[] { 0x8, 0x3 } : [0x3],
            BatteryState.Normal => LastState == BatteryState.Conservation ? [0x5] : [0x8],
            BatteryState.RapidCharge => LastState == BatteryState.Conservation ? [0x5, 0x7] : [0x7],
            _ => throw new InvalidOperationException("Invalid state")
        };
        return Task.FromResult(result);
    }

    protected override Task<BatteryState> FromInternalAsync(uint state)
    {
        state = state.ReverseEndianness();

        if (state.GetNthBit(17)) // Is charging?
            return Task.FromResult(state.GetNthBit(26) ? BatteryState.RapidCharge : BatteryState.Normal);

        if (state.GetNthBit(29))
            return Task.FromResult(BatteryState.Conservation);

        throw new InvalidOperationException($"Unknown battery state: {state} [bits={Convert.ToString(state, 2)}]");
    }

    public override async Task SetStateAsync(BatteryState state)
    {
        await base.SetStateAsync(state).ConfigureAwait(false);
        SetStateInRegistry(state);
    }

    public async Task EnsureCorrectBatteryModeIsSetAsync()
    {
        var state = GetStateFromRegistry();

        if (!state.HasValue)
            return;

        if (await GetStateAsync().ConfigureAwait(false) == state.Value)
            return;

        await SetStateAsync(state.Value).ConfigureAwait(false);
    }

    private static BatteryState? GetStateFromRegistry()
    {
        var batteryModeString = Registry.GetValue(BATTERY_CHARGE_MODE_HIVE, BATTERY_CHARGE_MODE_PATH, BATTERY_CHARGE_MODE_KEY, string.Empty);
        return batteryModeString switch
        {
            BATTERY_CHARGE_MODE_NORMAL => BatteryState.Normal,
            BATTERY_CHARGE_MODE_RAPID_CHARGE => BatteryState.RapidCharge,
            BATTERY_CHARGE_MODE_CONSERVATION => BatteryState.Conservation,
            _ => null
        };
    }

    private static void SetStateInRegistry(BatteryState state)
    {
        var batteryModeString = state switch
        {
            BatteryState.Normal => BATTERY_CHARGE_MODE_NORMAL,
            BatteryState.RapidCharge => BATTERY_CHARGE_MODE_RAPID_CHARGE,
            BatteryState.Conservation => BATTERY_CHARGE_MODE_CONSERVATION,
            _ => null
        };

        if (batteryModeString is null)
            return;

        Registry.SetValue(BATTERY_CHARGE_MODE_HIVE, BATTERY_CHARGE_MODE_PATH, BATTERY_CHARGE_MODE_KEY, batteryModeString);
    }
}
