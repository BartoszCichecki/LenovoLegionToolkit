using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features
{
    public interface IFeature<T> where T : struct
    {
        public Task<T[]> GetAllStatesAsync();
        Task<T> GetStateAsync();
        Task SetStateAsync(T state);
    }
}
