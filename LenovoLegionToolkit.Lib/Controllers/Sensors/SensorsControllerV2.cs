namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public class SensorsControllerV2 : AbstractSensorsController
{
    public SensorsControllerV2(GPUController gpuController)
        : base(new() { CPUSensorID = 3, GPUSensorID = 4, CPUFanID = 0, GPUFanID = 1 }, gpuController)
    {
    }
}