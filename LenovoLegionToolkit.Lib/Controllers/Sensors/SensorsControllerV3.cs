namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV3 : AbstractSensorsController
{
    protected override SensorSettings Settings => new()
    {
        CPUSensorID = 4,
        GPUSensorID = 5,
        CPUFanID = 1,
        GPUFanID = 2
    };
}
