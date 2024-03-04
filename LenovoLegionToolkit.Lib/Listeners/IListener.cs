using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners;

public interface IListener<TEventArgs> where TEventArgs : EventArgs
{
    event EventHandler<TEventArgs>? Changed;

    Task StartAsync();

    Task StopAsync();
}
