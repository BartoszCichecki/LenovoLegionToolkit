using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Extensions;

public static class SymbolRegularExtensions
{
    public static IconElement GetIcon(this SymbolRegular sr) => new SymbolIcon(sr);
}
