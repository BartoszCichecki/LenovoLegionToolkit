using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T value)
                {
                    yield return value;
                }
                else
                {
                    foreach (var sub in GetChildrenOfType<T>(child))
                        yield return sub;
                }
            }
        }
    }
}
