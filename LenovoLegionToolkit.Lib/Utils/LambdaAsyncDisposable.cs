using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils;

public class LambdaAsyncDisposable : IAsyncDisposable
{
    private readonly Func<Task> _action;

    public LambdaAsyncDisposable(Func<Task> action) => _action = action;

    public async ValueTask DisposeAsync() => await _action();
}
