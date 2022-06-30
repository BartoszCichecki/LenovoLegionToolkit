using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Windows;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class SnackbarHelper
    {
        public static async Task ShowAsync(string title, string message, bool isError = false)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var snackBar = mainWindow?.Snackbar;

            if (snackBar is null)
                return;

            snackBar.Icon = isError ? SymbolRegular.ErrorCircle24 : SymbolRegular.Checkmark24;
            await snackBar.ShowAsync(title, message);
        }

        public static void Show(string title, string message, bool isError = false)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var snackBar = mainWindow?.Snackbar;

            if (snackBar is null)
                return;

            snackBar.Icon = isError ? SymbolRegular.ErrorCircle24 : SymbolRegular.Checkmark24;
            snackBar.Show(title, message);
        }
    }
}
