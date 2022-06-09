using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFUI.Common;
using MessageBox = WPFUI.Controls.MessageBox;
using TextBox = WPFUI.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class MessageBoxHelper
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

        public static Task<string?> ShowInputAsync(
            DependencyObject dependencyObject,
            string title,
            string? placeholder = null,
            string? text = null,
            string primaryButton = "OK",
            string secondaryButton = "Cancel",
            bool allowEmpty = false
        )
        {
            return ShowInputAsync(Window.GetWindow(dependencyObject), title, placeholder, text, primaryButton, secondaryButton, allowEmpty);
        }

        public static Task<string?> ShowInputAsync(
            Window window,
            string title,
            string? placeholder = null,
            string? text = null,
            string primaryButton = "OK",
            string secondaryButton = "Cancel",
            bool allowEmpty = false
        )
        {
            var tcs = new TaskCompletionSource<string?>();

            var textBox = new TextBox
            {
                MaxLines = 1,
                MaxLength = 50,
                Placeholder = placeholder,
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                SelectionStart = text?.Length ?? 0,
                SelectionLength = 0
            };
            var messageBox = new MessageBox
            {
                Owner = window,
                Title = title,
                Content = textBox,
                ButtonLeftAppearance = Appearance.Transparent,
                ButtonLeftName = primaryButton,
                ButtonRightName = secondaryButton,
                ShowInTaskbar = false,
                Topmost = false,
                MinHeight = 160,
                MaxHeight = 160,
                ResizeMode = ResizeMode.NoResize,
            };

            textBox.TextChanged += (s, e) =>
            {
                var isEmpty = !allowEmpty && string.IsNullOrWhiteSpace(textBox.Text);
                messageBox.ButtonLeftAppearance = isEmpty ? Appearance.Transparent : Appearance.Primary;
            };
            messageBox.ButtonLeftClick += (s, e) =>
            {
                var content = textBox.Text?.Trim();
                var text = string.IsNullOrWhiteSpace(content) ? null : content;
                if (!allowEmpty && text is null)
                    return;
                tcs.SetResult(text);
                messageBox.Close();
            };
            messageBox.ButtonRightClick += (s, e) =>
            {
                tcs.SetResult("");
                messageBox.Close();
            };
            messageBox.Closing += (s, e) =>
            {
                tcs.TrySetResult("");
            };
            messageBox.Show();

            FocusManager.SetFocusedElement(window, textBox);

            return tcs.Task;
        }
    }
}
