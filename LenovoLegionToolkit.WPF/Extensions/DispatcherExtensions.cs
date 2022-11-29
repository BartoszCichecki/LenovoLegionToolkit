using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LenovoLegionToolkit.WPF.Extensions;

public static class DispatcherExtensions
{
    public static void InvokeTask(this Dispatcher dispatcher, Func<Task> action) => dispatcher.Invoke(async () => await action());
}