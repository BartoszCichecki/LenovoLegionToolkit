using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Windows;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Utils;

public static class SnackbarHelper
{
    public static async Task ShowAsync(string title, string message, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        snackBar.Icon = type switch
        {
            SnackbarType.Warning => SymbolRegular.Warning24,
            SnackbarType.Error => SymbolRegular.ErrorCircle24,
            _ => SymbolRegular.Checkmark24
        };
        snackBar.Timeout = type switch
        {
            SnackbarType.Warning => 5000,
            SnackbarType.Error => 5000,
            _ => 2000
        };
        snackBar.CloseButtonEnabled = type switch
        {
            SnackbarType.Warning => true,
            SnackbarType.Error => true,
            _ => false
        };
        await snackBar.ShowAsync(title, message);
    }

    public static void Show(string title, string message, SnackbarType type = SnackbarType.Success)
    {
        var mainWindow = Application.Current.MainWindow as MainWindow;
        var snackBar = mainWindow?.Snackbar;

        if (snackBar is null)
            return;

        snackBar.Icon = type switch
        {
            SnackbarType.Warning => SymbolRegular.Warning24,
            SnackbarType.Error => SymbolRegular.ErrorCircle24,
            _ => SymbolRegular.Checkmark24
        };
        snackBar.Timeout = type switch
        {
            SnackbarType.Warning => 5000,
            SnackbarType.Error => 5000,
            _ => 2000
        };
        snackBar.CloseButtonEnabled = type switch
        {
            SnackbarType.Warning => true,
            SnackbarType.Error => true,
            _ => false
        };
        snackBar.Show(title, message);
    }
}