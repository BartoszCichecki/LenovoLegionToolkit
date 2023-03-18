using System;

namespace LenovoLegionToolkit.Lib.Utils;

public class ThreadSafeCounter
{
    private readonly object _lock = new();

    private int _counter;

    public bool Decrement()
    {
        lock (_lock)
        {
            var value = _counter < 1;
            _counter = Math.Max(0, _counter - 1);
            return value;
        }
    }

    public void Increment()
    {
        lock (_lock)
        {
            _counter++;
        }
    }
}
