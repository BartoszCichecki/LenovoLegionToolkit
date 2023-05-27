using System;

namespace LenovoLegionToolkit.Lib.Utils;

public interface IMainThreadDispatcher
{
    void Dispatch(Action callback);
}
