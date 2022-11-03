using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class UIElementExtensions
    {
        public static IEnumerable<T> GetVisibleChildrenOfType<T>(this UIElement depObj) where T : UIElement
        {
            if (depObj.Visibility != Visibility.Visible)
                yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T value)
                {
                    yield return value;
                }
                else if (child is UIElement element)
                {
                    foreach (var sub in GetVisibleChildrenOfType<T>(element))
                        yield return sub;
                }
            }
        }
    }
}
