using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using NeoSmart.AsyncLock;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Utils;

public class ContextMenuHelper
{
    private const string STATIC_TAG = "static";
    private const string NAVIGATION_TAG = "navigation";
    private const string QUICK_ACTIONS_TAG = "quickActions";

    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
    private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

    private readonly AsyncLock _refreshLock = new();

    public ContextMenu ContextMenu { get; }

    public Action? BringToForeground { get; set; }
    public Func<Task>? Close { get; set; }

    private bool _runOnce;

    public ContextMenuHelper()
    {
        ContextMenu = new ContextMenu
        {
            FontSize = 14
        };

        var openMenuItem = new MenuItem { Header = Resource.Open, Tag = STATIC_TAG };
        openMenuItem.Click += (_, _) => BringToForeground?.Invoke();

        var closeMenuItem = new MenuItem { Header = Resource.Close, Tag = STATIC_TAG };
        closeMenuItem.Click += async (_, _) =>
        {
            if (Close is not null)
                await Close.Invoke();
        };

        ContextMenu.Items.Add(openMenuItem);
        ContextMenu.Items.Add(closeMenuItem);

        ContextMenu.Loaded += MainContextMenu_Loaded;
    }

    private async void MainContextMenu_Loaded(object sender, RoutedEventArgs e)
    {
        if (_runOnce)
            return;

        _runOnce = true;

        var pipelines = await _automationProcessor.GetPipelinesAsync();
        await RefreshAutomationMenuItemsAsync(pipelines);

        _automationProcessor.PipelinesChanged += async (_, e) => await RefreshAutomationMenuItemsAsync(e);
        _themeManager.ThemeApplied += (_, _) => ContextMenu.Resources = Application.Current.Resources;
    }

    public void SetNavigationItems(NavigationStore navigationStore)
    {
        var items = new List<Control>();

        foreach (var item in ContextMenu.Items.ToArray().OfType<Control>().Where(mi => NAVIGATION_TAG.Equals(mi.Tag)))
            ContextMenu.Items.Remove(item);

        foreach (var item in navigationStore.Items.OfType<NavigationItem>())
        {
            var menuItem = new MenuItem
            {
                SymbolIcon = item.Icon,
                Header = item.Content,
                Tag = NAVIGATION_TAG
            };
            menuItem.Click += async (_, _) =>
            {
                ContextMenu.IsOpen = false;
                BringToForeground?.Invoke();
                await Task.Delay(500); // Give window time to come back
                navigationStore.Navigate(item.PageTag);
            };
            items.Insert(0, menuItem);
        }

        if (items.Any())
            items.Insert(0, new Separator { Tag = NAVIGATION_TAG });

        foreach (var item in items)
            ContextMenu.Items.Insert(0, item);
    }

    public async Task RefreshAutomationMenuItemsAsync()
    {
        await RefreshAutomationMenuItemsAsync(await _automationProcessor.GetPipelinesAsync());
    }

    private async Task RefreshAutomationMenuItemsAsync(List<AutomationPipeline> pipelines)
    {
        using (await _refreshLock.LockAsync())
        {
            foreach (var item in ContextMenu.Items.ToArray().OfType<Control>().Where(mi => QUICK_ACTIONS_TAG.Equals(mi.Tag)))
                ContextMenu.Items.Remove(item);

            var items = new List<Control>
            {
                new MenuItem { Header = Resource.ContextMenu_QuickActions, Tag = QUICK_ACTIONS_TAG, IsEnabled = false }
            };

            foreach (var menuPipeline in pipelines.Where(p => p.Trigger is null))
            {
                var item = new MenuItem
                {
                    SymbolIcon = SymbolRegular.Play24,
                    Header = menuPipeline.Name ?? Resource.Unnamed,
                    Tag = QUICK_ACTIONS_TAG,
                };
                item.Click += async (_, _) =>
                {
                    try
                    {
                        await _automationProcessor.RunNowAsync(menuPipeline);
                    }
                    catch (Exception) { }
                };

                items.Insert(0, item);
            }

            items.Insert(0, new Separator { Tag = QUICK_ACTIONS_TAG });

            if (items.Count < 3)
                return;

            foreach (var item in items)
                ContextMenu.Items.Insert(0, item);
        }
    }
}