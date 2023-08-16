using System.Windows.Media;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Extensions;

public static class PowerModeStateExtensions
{
    public static SolidColorBrush GetSolidColorBrush(this PowerModeState powerModeState) => new(powerModeState switch
    {
        PowerModeState.Quiet => Color.FromRgb(53, 123, 242),
        PowerModeState.Balance => Colors.White,
        PowerModeState.Performance => Color.FromRgb(212, 51, 51),
        PowerModeState.GodMode => Color.FromRgb(99, 52, 227),
        _ => Colors.Transparent,
    });
}
