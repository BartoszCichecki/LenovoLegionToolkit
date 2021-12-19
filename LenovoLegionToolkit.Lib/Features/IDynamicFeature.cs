using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features
{
    public interface IDynamicFeature<T> : IFeature<T>
    {
        public Task<T[]> GetAllStatesAsync();
    }
}
