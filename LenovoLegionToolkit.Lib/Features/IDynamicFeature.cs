namespace LenovoLegionToolkit.Lib.Features
{
    public interface IDynamicFeature<T> : IFeature<T>
    {
        public T[] GetAllStates();
    }
}
