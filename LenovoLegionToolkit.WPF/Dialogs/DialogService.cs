using System.Diagnostics.CodeAnalysis;
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

            var dialog = new Dialog
            {
                Content = stackPanel,
                ButtonLeftName = leftButton,
                ButtonRightName = rightButton,
                ButtonLeftAppearance = destructive ? Appearance.Danger : Appearance.Primary,
                DialogHeight = 300,
            };
            dialog.ButtonLeftClick += (s, e) =>
            {
                tcs.SetResult(true);
                dialog.Hide();
            };
            dialog.ButtonRightClick += (s, e) =>
            {
                tcs.SetResult(false);
                dialog.Hide();
            };

            ShowDialog(dialog);

            return tcs.Task;
        }


        private static void ShowDialog(Dialog dialog)
        {
            var mainContent = ((Grid)Application.Current.MainWindow.Content).Children;

            dialog.Closed += ([NotNull] s, e) =>
            {
                mainContent.Remove(dialog);
            };

            Grid.SetRow(dialog, 0);
            Grid.SetRowSpan(dialog, 2);

            mainContent.Add(dialog);

            dialog.Show();
        }
    }
}
