using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

public interface ISensorsController
{
    Task<bool> IsSupportedAsync();
    Task<SensorsData> GetDataAsync();
}
