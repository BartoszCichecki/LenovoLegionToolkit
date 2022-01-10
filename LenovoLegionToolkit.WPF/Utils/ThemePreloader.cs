using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class ThemePreloader : Window, IDisposable
    {
        private readonly Type[] _types = {
            // WPF
            typeof(System.Windows.Controls.Button),
            typeof(System.Windows.Controls.ComboBox),
            typeof(System.Windows.Controls.Grid),
            typeof(System.Windows.Controls.StackPanel),
            typeof(System.Windows.Controls.TextBlock),
            typeof(System.Windows.Controls.Primitives.ToggleButton),

            // WPFUI
            typeof(WPFUI.Controls.Button),
            typeof(WPFUI.Controls.CardAction),
            typeof(WPFUI.Controls.CardControl),
            typeof(WPFUI.Controls.Hyperlink),
            typeof(WPFUI.Controls.NavigationItem),
            typeof(WPFUI.Controls.NavigationStore),
            typeof(WPFUI.Controls.TitleBar),
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
            var stackPanel = new System.Windows.Controls.StackPanel();

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
