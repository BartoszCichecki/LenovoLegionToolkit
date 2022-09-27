using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public interface IListener<T>
    {
        event EventHandler<T>? Changed;

        Task StartAsync();

        Task StopAsync();
    }

    public interface INotifyingListener<T> : IListener<T>
    {
        Task NotifyAsync(T value);
    }
}