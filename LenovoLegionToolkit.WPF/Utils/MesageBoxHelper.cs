using System.Threading.Tasks;
using System.Windows;
using WPFUI.Common;
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
            string rightButton = "No",
            bool destructive = false
        )
        {
            return ShowAsync(Window.GetWindow(dependencyObject), title, message, leftButton, rightButton, destructive);
        }

        public static Task<bool> ShowAsync(
            Window window,
            string title,
            string message,
            string leftButton = "Yes",
            string rightButton = "No",
            bool destructive = false
        )
        {
            var tcs = new TaskCompletionSource<bool>();

            var messageBox = new MessageBox
            {
                Owner = window,
                Title = title,
                Content = message,
                ButtonLeftName = leftButton,
                ButtonRightName = rightButton,
                ButtonLeftAppearance = destructive ? Appearance.Danger : Appearance.Primary,
                ShowInTaskbar = false,
            };
            messageBox.ButtonLeftClick += (s, e) =>
            {
                messageBox.Close();
                tcs.SetResult(true);
            };
            messageBox.ButtonRightClick += (s, e) =>
            {
                messageBox.Close();
                tcs.SetResult(false);
            };
            messageBox.Show();

            return tcs.Task;
        }
    }
}
