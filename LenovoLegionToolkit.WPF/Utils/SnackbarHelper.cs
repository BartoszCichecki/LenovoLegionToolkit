using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.WPF.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils;

public static class SnackbarHelper
{
    public static async Task ShowAsync(string title, string? message = null, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        SetupSnackbarAppearance(snackBar, title, message, type);
        SetTitleAndMessage(snackBar, title, message);
        await snackBar.HideAsync();
        await snackBar.ShowAsync();
    }

    public static void Show(string title, string? message = null, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        SetupSnackbarAppearance(snackBar, title, message, type);
        SetTitleAndMessage(snackBar, title, message);
        snackBar.Show();
    }

    private static void SetupSnackbarAppearance(Snackbar snackBar, string title, string? message, SnackbarType type)
    {
        snackBar.Appearance = type switch
        {
            SnackbarType.Warning => ControlAppearance.Caution,
            SnackbarType.Error => ControlAppearance.Danger,
            _ => ControlAppearance.Secondary
        };
        snackBar.Icon = type switch
        {
            SnackbarType.Warning => SymbolRegular.Warning24,
            SnackbarType.Error => SymbolRegular.ErrorCircle24,
            SnackbarType.Info => SymbolRegular.Info24,
            _ => SymbolRegular.Checkmark24
        };
        snackBar.Timeout = type switch
        {
            SnackbarType.Success => 2000,
            _ => Math.Clamp(GetTextLengthInMilliseconds(title, message), 5000, 10000)
        };
        snackBar.CloseButtonEnabled = type switch
        {
            SnackbarType.Success => false,
            _ => true
        };
    }

    private static void SetTitleAndMessage(FrameworkElement snackBar, string title, string? message)
    {
        if (snackBar.FindName("_snackbarTitle") is TextBlock snackbarTitle)
            snackbarTitle.Text = title;

        if (snackBar.FindName("_snackbarMessage") is TextBlock snackbarMessage)
        {
            snackbarMessage.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            snackbarMessage.Text = message;
        }
    }

    private static int GetTextLengthInMilliseconds(string title, string? message)
    {
        var length = 2 + (title.Length + (message?.Length ?? 0)) % 10;
        return length * 1000;
    }
}
