using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.WPF.Assets;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Utils;

public class TrayHelper : IDisposable
{
    private const string NAVIGATION_TAG = "navigation";
    private const string STATIC_TAG = "static";
    private const string AUTOMATION_TAG = "automation";

    private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

    private readonly ContextMenu _contextMenu = new()
    {
        FontSize = 14
    };

    private readonly Action _bringToForeground;

    private NotifyIcon? _notifyIcon;

    public TrayHelper(INavigation navigation, Action bringToForeground, bool trayTooltipEnabled)
    {
        _bringToForeground = bringToForeground;

        InitializeStaticItems(navigation);

        var notifyIcon = new NotifyIcon
        {
            Icon = AssetResources.icon,
            Text = Resource.AppName
        };

        if (trayTooltipEnabled)
            notifyIcon.ToolTipWindow = async () => await StatusWindow.CreateAsync();

        notifyIcon.ContextMenu = _contextMenu;
        notifyIcon.OnClick += (_, _) => _bringToForeground();
        _notifyIcon = notifyIcon;

        _themeManager.ThemeApplied += (_, _) => _contextMenu.Resources = App.Current.Resources;
    }

    public async Task InitializeAsync()
    {
        var pipelines = await _automationProcessor.GetPipelinesAsync();
        pipelines = pipelines.Where(p => p.Trigger is null).ToList();
        SetAutomationItems(pipelines);

        _automationProcessor.PipelinesChanged += (_, p) => SetAutomationItems(p);
    }

    private void InitializeStaticItems(INavigation navigation)
    {
        foreach (var navigationItem in navigation.Items.OfType<NavigationItem>())
        {
            var navigationMenuItem = new MenuItem
            {
                SymbolIcon = navigationItem.Icon,
                Header = navigationItem.Content,
                Tag = NAVIGATION_TAG
            };
            navigationMenuItem.Click += async (_, _) =>
            {
                _contextMenu.IsOpen = false;
                _bringToForeground();

                await Task.Delay(TimeSpan.FromMilliseconds(500));
                navigation.Navigate(navigationItem.PageTag);
            };
            _contextMenu.Items.Add(navigationMenuItem);
        }

        _contextMenu.Items.Add(new Separator { Tag = NAVIGATION_TAG });

        var openMenuItem = new MenuItem { Header = Resource.Open, Tag = STATIC_TAG };
        openMenuItem.Click += (_, _) =>
        {
            _contextMenu.IsOpen = false;
            _bringToForeground();
        };
        _contextMenu.Items.Add(openMenuItem);

        var closeMenuItem = new MenuItem { Header = Resource.Close, Tag = STATIC_TAG };
        closeMenuItem.Click += async (_, _) =>
        {
            _contextMenu.IsOpen = false;
            await App.Current.ShutdownAsync();
        };
        _contextMenu.Items.Add(closeMenuItem);
    }

    private void SetAutomationItems(List<AutomationPipeline> pipelines)
    {
        foreach (var item in _contextMenu.Items.OfType<Control>().Where(mi => AUTOMATION_TAG.Equals(mi.Tag)).ToArray())
            _contextMenu.Items.Remove(item);

        pipelines = pipelines.Where(p => p.Trigger is null).Reverse().ToList();

        if (pipelines.Count != 0)
            _contextMenu.Items.Insert(0, new Separator { Tag = AUTOMATION_TAG });

        foreach (var pipeline in pipelines)
        {
            var icon = Enum.TryParse<SymbolRegular>(pipeline.IconName, out var iconParsed)
                ? iconParsed
                : SymbolRegular.Play24;

            var item = new MenuItem
            {
                SymbolIcon = icon,
                Header = pipeline.Name ?? Resource.Unnamed,
                Tag = AUTOMATION_TAG
            };
            item.Click += async (_, _) =>
            {
                try
                {
                    await _automationProcessor.RunNowAsync(pipeline);
                }
                catch {  /* Ignored. */ }
            };

            _contextMenu.Items.Insert(0, item);
        }
    }

    public void MakeVisible()
    {
        if (_notifyIcon is null)
            return;

        _notifyIcon.Visible = true;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_notifyIcon is not null)
            _notifyIcon.Visible = false;

        _notifyIcon?.Dispose();
        _notifyIcon = null;
    }
}
