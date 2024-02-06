using System.Linq;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Extensions;

public static class NavigationViewExtensions
{
    public static void NavigateToNext(this NavigationView navigationStore)
    {
        var navigationItems = navigationStore.MenuItems.OfType<INavigationViewItem>().ToList();
        var current = navigationStore.SelectedItem ?? navigationItems.FirstOrDefault();

        if (current is null)
            return;

        var index = (navigationItems.IndexOf(current) + 1) % navigationItems.Count;
        var next = navigationItems[index];

        navigationStore.Navigate(next.TargetPageTag);
    }

    public static void NavigateToPrevious(this NavigationView navigationStore)
    {
        var navigationItems = navigationStore.MenuItems.OfType<INavigationViewItem>().ToList();
        var current = navigationStore.SelectedItem ?? navigationItems.FirstOrDefault();

        if (current is null)
            return;

        var index = navigationItems.IndexOf(current) - 1;
        if (index < 0)
            index = navigationItems.Count - 1;
        var next = navigationItems[index];

        navigationStore.Navigate(next.TargetPageTag);
    }
}
