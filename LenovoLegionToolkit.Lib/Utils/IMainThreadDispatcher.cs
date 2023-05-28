using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils;

public interface IMainThreadDispatcher
{
    void Dispatch(Action callback);

    Task DispatchAsync(Func<Task> callback);
}
