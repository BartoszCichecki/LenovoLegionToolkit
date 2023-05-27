using System;
using System.Windows;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Utils;

public class MainThreadDispatcher : IMainThreadDispatcher
{
    public void Dispatch(Action callback) => Application.Current.Dispatcher.Invoke(callback);
}
