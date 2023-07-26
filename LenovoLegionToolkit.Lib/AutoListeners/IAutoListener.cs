using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.AutoListeners;

public interface IAutoListener<T>
{
    Task SubscribeChangedAsync(EventHandler<T> eventHandler);
    Task UnsubscribeChangedAsync(EventHandler<T> eventHandler);
}
