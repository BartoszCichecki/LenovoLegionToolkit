using System;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public interface IListener<T>
    {
        event EventHandler<T>? Changed;

        internal void Start();
    }
}