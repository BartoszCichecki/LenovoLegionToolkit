namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV2 : AbstractSensorsController
{
    protected override SensorSettings Settings => new()
    {
        CPUSensorID = 3,
        GPUSensorID = 4,
        CPUFanID = 0,
        GPUFanID = 1
    };
}