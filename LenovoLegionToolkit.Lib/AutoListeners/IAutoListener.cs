using System;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public interface IAutoListener<T>
{
    event EventHandler<T>? Changed;
}
