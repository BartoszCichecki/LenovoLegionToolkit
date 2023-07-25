using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners;

public interface INotifyingListener<T> : IListener<T>
{
    Task NotifyAsync(T value);
}
