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
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ContextMenuHelper
    {
        private static readonly string StaticTag = "static";
        private static readonly string ActionsTag = "actions";

        private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
        private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();

        private readonly AsyncLock _refreshLock = new();

        public ContextMenu ContextMenu { get; }

        public Action? BringToForegroundAction { get; set; }

        private static ContextMenuHelper? _instance;

        public static ContextMenuHelper Instance => _instance ??= new ContextMenuHelper();

        private ContextMenuHelper()
        {
            ContextMenu = new ContextMenu();

            var openMenuItem = new MenuItem { Header = "Open", Tag = StaticTag };
            openMenuItem.Click += (s, e) => BringToForegroundAction?.Invoke();

            var closeMenuItem = new MenuItem { Header = "Close", Tag = StaticTag };
            closeMenuItem.Click += (s, e) => Application.Current.Shutdown();

            ContextMenu.Items.Add(openMenuItem);
            ContextMenu.Items.Add(closeMenuItem);

            ContextMenu.Loaded += MainContextMenu_Loaded;
        }

        private async void MainContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            var pipelines = await _automationProcessor.GetPipelinesAsync();
            await RefreshAutomationMenuItemsAsync(pipelines);

            _automationProcessor.PipelinesChanged += async (s, e) => await RefreshAutomationMenuItemsAsync(e.Pipelines);
            _themeManager.ThemeApplied += async (s, e) =>
            {
                var pipelines = await _automationProcessor.GetPipelinesAsync();
                await RefreshAutomationMenuItemsAsync(pipelines);
            };
        }

        private async Task RefreshAutomationMenuItemsAsync(List<AutomationPipeline> pipelines)
        {
            using (await _refreshLock.LockAsync())
            {
                foreach (var item in ContextMenu.Items.ToArray().OfType<Control>().Where(mi => ActionsTag.Equals(mi.Tag)))
                    ContextMenu.Items.Remove(item);

                var items = new List<Control>
                {
                    new MenuItem { Header = "Actions", Tag = ActionsTag, IsEnabled = false }
                };

                foreach (var menuPipeline in pipelines.Where(p => p.Triggers.IsEmpty()))
                {
                    var item = new MenuItem
                    {
                        Header = menuPipeline.Name ?? "Unnamed",
                        Tag = ActionsTag,
                    };
                    item.Click += async (s, e) => await _automationProcessor.RunNowAsync(menuPipeline);
                    items.Insert(0, item);
                }

                if (items.Any())
                    items.Insert(0, new Separator { Tag = ActionsTag });

                foreach (var item in items)
                    ContextMenu.Items.Insert(0, item);
            }
        }
    }
}
