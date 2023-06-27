namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV3 : AbstractSensorsController
{
    public SensorsControllerV3(GPUController gpuController)
        : base(new() { CPUSensorID = 4, GPUSensorID = 5, CPUFanID = 1, GPUFanID = 2 }, gpuController)
    {
    }
}
