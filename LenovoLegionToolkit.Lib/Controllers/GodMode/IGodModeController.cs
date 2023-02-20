using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Controllers.GodMode;

public interface IGodModeController
{
    Task<GodModeState> GetStateAsync();
    Task SetStateAsync(GodModeState state);
    Task ApplyStateAsync();
}
