using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Lib.Controllers;

public class AIChipController
{
    private readonly AIModeController _aiModeController;
    private readonly BalanceModeSettings _settings;

    public bool IsEnabled
    {
        get => _settings.Store.AIChipEnabled;
        set
        {
            _settings.Store.AIChipEnabled = value;
            _settings.SynchronizeStore();
        }
    }
    public AIChipController(AIModeController aiModeController, BalanceModeSettings settings)
    {
        _aiModeController = aiModeController ?? throw new ArgumentNullException(nameof(aiModeController));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    private static async Task<bool> IsSupportedAsync()
    {
        var mi = await Compatibility.GetMachineInformationAsync().ConfigureAwait(false);
        return mi.Features.AIChip;
    }
}