using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = WPFUI.Controls.MessageBox;

namespace LenovoLegionToolkit.WPF.Utils
{
    internal class MessageBoxHelper
    {
        public static Task<bool> ShowAsync(
            DependencyObject dependencyObject,
            string title,
            string message,
            string leftButton = "Yes",
            string rightButton = "No"
        )
        {
            return ShowAsync(Window.GetWindow(dependencyObject), title, message, leftButton, rightButton);
        }

        public static Task<bool> ShowAsync(
            Window window,
            string title,
            string message,
            string primaryButton = "Yes",
            string secondaryButton = "No"
        )
        {
            var tcs = new TaskCompletionSource<bool>();

            var messageBox = new MessageBox
            {
                Owner = window,
                Title = title,
                Content = new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                },
                ButtonLeftName = primaryButton,
                ButtonRightName = secondaryButton,
                ShowInTaskbar = false,
                Topmost = false,
                ResizeMode = ResizeMode.NoResize,
            };
            messageBox.ButtonLeftClick += (s, e) =>
            {
                tcs.SetResult(true);
                messageBox.Close();
            };
            messageBox.ButtonRightClick += (s, e) =>
            {
                tcs.SetResult(false);
                messageBox.Close();
            };
            messageBox.Closing += (s, e) =>
            {
                tcs.TrySetResult(false);
            };
            messageBox.Show();

            return tcs.Task;
        }
    }
}
