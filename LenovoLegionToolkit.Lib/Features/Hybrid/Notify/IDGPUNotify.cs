using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Features.Hybrid.Notify;

public interface IDGPUNotify
{
    Task<bool> IsSupportedAsync();
    Task NotifyAsync();
}
