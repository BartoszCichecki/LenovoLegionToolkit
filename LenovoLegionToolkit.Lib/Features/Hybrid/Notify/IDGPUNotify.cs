using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public interface IDGPUNotify
{
    public event EventHandler<bool>? Notified;

    Task<bool> IsSupportedAsync();
    Task NotifyAsync();
}
