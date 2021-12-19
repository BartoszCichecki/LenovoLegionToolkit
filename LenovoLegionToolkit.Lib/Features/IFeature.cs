using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features
{
    public interface IFeature<T>
    {
        Task<T> GetStateAsync();
        Task SetStateAsync(T state);
    }
}