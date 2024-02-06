using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Windows;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils;

public static class SnackbarHelper
{
    public static async Task ShowAsync(string title, string? message = null, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBarPresenter = mainWindow?.SnackbarPresenter;
        if (snackBarPresenter is null)
            return;

        var snackBar = GetSnackBar(snackBarPresenter, type, title, message);
        await snackBar.ShowAsync();
    }

    public static void Show(string title, string? message = null, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBarPresenter = mainWindow?.SnackbarPresenter;
        if (snackBarPresenter is null)
            return;

        var snackBar = GetSnackBar(snackBarPresenter, type, title, message);
        snackBar.Show();
    }

    private static Snackbar GetSnackBar(SnackbarPresenter snackBarPresenter, SnackbarType type, string title, string? message)
    {
        var snackBar = new Snackbar(snackBarPresenter)
        {
            Appearance = type switch
            {
                SnackbarType.Warning => ControlAppearance.Caution,
                SnackbarType.Error => ControlAppearance.Danger,
                _ => ControlAppearance.Secondary
            },
            Icon = type switch
            {
                SnackbarType.Warning => SymbolRegular.Warning24.GetIcon(),
                SnackbarType.Error => SymbolRegular.ErrorCircle24.GetIcon(),
                SnackbarType.Info => SymbolRegular.Info24.GetIcon(),
                _ => SymbolRegular.Checkmark24.GetIcon()
            },
            Title = title,
            Content = message,
            IsCloseButtonEnabled = type switch
            {
                SnackbarType.Success => false,
                _ => true
            },
            Timeout = type switch
            {
                SnackbarType.Success => TimeSpan.FromSeconds(2),
                _ => GetTimeSpanForTextLength(title, message)
            }
        };
        return snackBar;
    }

    private static TimeSpan GetTimeSpanForTextLength(string title, string? message)
    {
        var length = 2 + (title.Length + (message?.Length ?? 0)) % 10;
        var milliseconds = Math.Clamp(length * 1000, 5000, 10000);
        return TimeSpan.FromMilliseconds(milliseconds);
    }
}
