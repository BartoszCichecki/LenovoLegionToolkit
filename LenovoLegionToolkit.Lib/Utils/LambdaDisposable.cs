using System;

namespace LenovoLegionToolkit.Lib.Utils;

public class LambdaDisposable(Action action) : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        action();
    }
}
