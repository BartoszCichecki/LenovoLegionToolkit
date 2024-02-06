using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;
using TextBlock = System.Windows.Controls.TextBlock;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Utils;

public static class MessageBoxHelper
{
    public static Task<bool> ShowAsync(DependencyObject dependencyObject,
        string title,
        string message,
        string? leftButton = null,
        string? rightButton = null
    )
    {
        var window = Window.GetWindow(dependencyObject)
                     ?? Application.Current.MainWindow
                     ?? throw new InvalidOperationException("Cannot show message without window.");
        return ShowAsync(window, title, message, leftButton, rightButton);
    }

    public static async Task<bool> ShowAsync(Window window,
        string title,
        string message,
        string? primaryButton = null,
        string? secondaryButton = null)
    {
        var messageBox = new MessageBox
        {
            Owner = window,
            Title = title,
            Content = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
            },

            PrimaryButtonText = primaryButton ?? Resource.Yes,
            SecondaryButtonText = secondaryButton ?? Resource.No,
            ShowInTaskbar = false,
            Topmost = false,
            ResizeMode = ResizeMode.NoResize,
        };
        var result = await messageBox.ShowDialogAsync();
        return result == MessageBoxResult.Primary;
    }

    public static Task<string?> ShowInputAsync(
        DependencyObject dependencyObject,
        string title,
        string? placeholder = null,
        string? text = null,
        string? primaryButton = null,
        string? secondaryButton = null,
        bool allowEmpty = false
    )
    {
        var window = Window.GetWindow(dependencyObject)
                     ?? Application.Current.MainWindow
                     ?? throw new InvalidOperationException("Cannot show message without window.");
        return ShowInputAsync(window, title, placeholder, text, primaryButton, secondaryButton, allowEmpty);
    }

    public static async Task<string?> ShowInputAsync(
        Window window,
        string title,
        string? placeholder = null,
        string? text = null,
        string? primaryButton = null,
        string? secondaryButton = null,
        bool allowEmpty = false
    )
    {
        var tcs = new TaskCompletionSource<string?>();

        var textBox = new TextBox
        {
            MaxLines = 1,
            MaxLength = 50,
            PlaceholderText = placeholder ?? string.Empty,
            TextWrapping = TextWrapping.Wrap
        };
        var messageBox = new MessageBox
        {
            Owner = window,
            Title = title,
            Content = textBox,
            PrimaryButtonAppearance = ControlAppearance.Transparent,
            PrimaryButtonText = primaryButton ?? Resource.OK,
            SecondaryButtonText = secondaryButton ?? Resource.Cancel,
            ShowInTaskbar = false,
            Topmost = false,
            MinHeight = 160,
            MaxHeight = 160,
            ResizeMode = ResizeMode.NoResize,
        };

        textBox.TextChanged += (_, _) =>
        {
            var isEmpty = !allowEmpty && string.IsNullOrWhiteSpace(textBox.Text);
            messageBox.PrimaryButtonAppearance = isEmpty ? ControlAppearance.Transparent : ControlAppearance.Primary;
        };

        var task = messageBox.ShowDialogAsync();

        textBox.Text = text ?? string.Empty;
        textBox.SelectionStart = text?.Length ?? 0;
        textBox.SelectionLength = 0;

        FocusManager.SetFocusedElement(window, textBox);

        var result = await task;

        if (result == MessageBoxResult.Primary)
        {
            var content = textBox.Text.Trim();
            var newText = string.IsNullOrWhiteSpace(content) ? null : content;
            if (!allowEmpty && newText is null)
                return null;
            return newText;
        }

        return null;
    }
}
