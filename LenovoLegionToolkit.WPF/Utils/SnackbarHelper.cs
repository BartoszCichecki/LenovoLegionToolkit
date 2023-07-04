using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Utils;

public static class SnackbarHelper
{
    public static async Task ShowAsync(string title, string message, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        SetupSnackbarAppearance(snackBar, type);
        await snackBar.ShowAsync(title, message);
    }

    public static void Show(string title, string message, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        SetupSnackbarAppearance(snackBar, type);
        snackBar.Show(title, message);
    }

    private static void SetupSnackbarAppearance(Snackbar snackBar, SnackbarType type)
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
            _ => SymbolRegular.Checkmark24
        };
        snackBar.Timeout = type switch
        {
            SnackbarType.Warning or SnackbarType.Error => 5000,
            _ => 2000
        };
        snackBar.CloseButtonEnabled = type switch
        {
            SnackbarType.Warning or SnackbarType.Error => true,
            _ => false
        };
    }
}
