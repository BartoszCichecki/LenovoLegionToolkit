using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFUI.Common;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Dialogs
{
    internal class DialogService
    {
        public static Task<bool> ShowDialogAsync(
            string title,
            string message,
            string leftButton = "Yes",
            string rightButton = "No",
            bool destructive = false
        )
        {
            var tcs = new TaskCompletionSource<bool>();

            var titleBlock = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Medium,
                FontSize = 24,
                Margin = new Thickness(0, 0, 0, 16),
            };

            var messageBlock = new TextBlock
            {
                Text = message,
                TextAlignment = TextAlignment.Justify,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(titleBlock);
            stackPanel.Children.Add(messageBlock);

            var mainContent = ((Grid)Application.Current.MainWindow.Content).Children;

            var dialog = new Dialog
            {
                Content = stackPanel,
                ButtonLeftName = leftButton,
                ButtonRightName = rightButton,
                ButtonLeftAppearance = destructive ? Appearance.Danger : Appearance.Primary,
                DialogHeight = 300,
                Show = true,
            };
            dialog.ButtonLeftClick += (s, e) =>
            {
                tcs.SetResult(true);
                mainContent.Remove(dialog);
            };
            dialog.ButtonRightClick += (s, e) =>
            {
                tcs.SetResult(false);
                mainContent.Remove(dialog);
            };

            Grid.SetRow(dialog, 0);
            Grid.SetRowSpan(dialog, 2);

            mainContent.Add(dialog);

            return tcs.Task;
        }
    }
}
