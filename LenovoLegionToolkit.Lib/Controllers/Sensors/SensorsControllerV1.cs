using System;
using System.Linq;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV1 : AbstractSensorsController
{
    protected override SensorSettings Settings => new()
    {
        CPUSensorID = 3,
        GPUSensorID = 4,
        CPUFanID = 0,
        GPUFanID = 1
    };

    public override async Task<bool> IsSupportedAsync()
    {
        try
        {
            var result = await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {Settings.CPUFanID}").ConfigureAwait(false);
            result &= await WMI.ExistsAsync("root\\WMI", $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {Settings.GPUFanID}").ConfigureAwait(false);
            return result;
        }
        catch
        {
            return false;
        }
    }

    protected override async Task<int> GetMaxFanSpeedAsync(int sensorID, int fanID)
    {
        var result = await WMI.ReadAsync("root\\WMI",
            $"SELECT * FROM LENOVO_FAN_TABLE_DATA WHERE Sensor_ID = 0 AND Fan_Id = {fanID}",
            pdc => Convert.ToInt32(pdc["DefaultFanMaxSpeed"].Value)).ConfigureAwait(false);
        return result.FirstOrDefault();
    }
}
