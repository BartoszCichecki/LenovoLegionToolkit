using System;

namespace LenovoLegionToolkit.Lib.Utils;

public class LambdaDisposable : IDisposable
{
    private readonly Action _action;

    public LambdaDisposable(Action action) => _action = action;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _action();
    }
}
