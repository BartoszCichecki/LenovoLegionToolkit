using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features;

public interface IFeature<T> where T : struct
{
    Task<bool> IsSupportedAsync();
    Task<T[]> GetAllStatesAsync();
    Task<T> GetStateAsync();
    Task SetStateAsync(T state);
}