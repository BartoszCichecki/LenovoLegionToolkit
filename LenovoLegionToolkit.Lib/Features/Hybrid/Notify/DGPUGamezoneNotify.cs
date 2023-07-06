using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public class DGPUGamezoneNotify : AbstractDGPUNotify
{
    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
            return mi is { Properties.SupportsIGPUMode: true };
        }
        catch
        {
            return false;
        }
    }

    protected override Task NotifyDGPUStatusAsync(bool state) => WMI.CallAsync("root\\WMI",
        $"SELECT * FROM LENOVO_OTHER_METHOD",
        "Set_DGPU_Device_Status",
        new() { { "Status", state ? 1 : 0 } });

    protected override async Task<HardwareId> GetDGPUHardwareIdAsync()
    {
        try
        {
            // ReSharper disable once StringLiteralTypo
            return await WMI.CallAsync("root\\WMI",
                $"SELECT * FROM LENOVO_GAMEZONE_DATA",
                "GetDGPUHWId",
                new(),
                pdc =>
                {
                    var id = pdc["Data"].Value.ToString();
                    return HardwareIdFromDGPUHardwareId(id);
                }).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return HardwareId.Empty;
        }
    }

    private static HardwareId HardwareIdFromDGPUHardwareId(string? gpuHwId)
    {
        try
        {
            if (gpuHwId is null)
                return default;

            var matches = new Regex("PCIVEN_([0-9A-F]{4})|DEV_([0-9A-F]{4})").Matches(gpuHwId);
            if (matches.Count != 2)
                return default;

            var vendor = matches[0].Groups[1].Value;
            var device = matches[1].Groups[2].Value;

            return new HardwareId { Vendor = vendor, Device = device };
        }
        catch
        {
            return default;
        }
    }
}
