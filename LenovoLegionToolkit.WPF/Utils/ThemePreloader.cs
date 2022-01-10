using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemePreloader : Window, IDisposable
    {
        private Type[] _types = {
            // WPF
            typeof(WPFUI.Controls.Button),
            typeof(ComboBox),
            typeof(Grid),
            typeof(StackPanel),
            typeof(TextBlock),
            typeof(ToggleButton),

            // WPFUI
            typeof(CardAction),
            typeof(CardControl),
            typeof(WPFUI.Controls.Hyperlink),
            typeof(NavigationItem),
            typeof(NavigationStore),
            typeof(TitleBar),
        };

        private TaskCompletionSource _taskCompletionSource = new();

        private ThemePreloader()
        {
            InitializeTypes();
            LayoutUpdated += Preload_LayoutUpdated;
        }

        private void Preload_LayoutUpdated(object sender, EventArgs e)
        {
            if (ActualHeight < 1 && ActualWidth < 1)
                return;

            LayoutUpdated -= Preload_LayoutUpdated;
            _taskCompletionSource.SetResult();
        }

        private void InitializeTypes()
        {
            var stackPanel = new StackPanel();

            _types.Select(Activator.CreateInstance)
                .OfType<UIElement>()
                .ToList()
                .ForEach(t => stackPanel.Children.Add(t));

            Content = stackPanel;
        }

        public static async Task<IDisposable> PreloadAsync()
        {
            var p = new ThemePreloader
            {
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = int.MaxValue,
                Top = int.MaxValue,
            };
            p.Show();
            await p._taskCompletionSource.Task;
            return p;
        }

        public void Dispose() => Close();
    }
}
