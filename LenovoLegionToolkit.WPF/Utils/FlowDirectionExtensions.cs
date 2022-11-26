using System.Windows;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class FlowDirectionExtensions
    {
        public static FlowDirection Direction => Resource.Culture.TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
    }
}
