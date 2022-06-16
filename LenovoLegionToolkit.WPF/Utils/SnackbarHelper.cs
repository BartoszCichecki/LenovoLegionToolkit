using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Windows;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class SnackbarHelper
    {
        public static async Task ShowAsync(string title, string message)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var snackBar = mainWindow?.Snackbar;

            if (snackBar is null)
                return;

            await snackBar.ShowAsync(title, message);
        }
    }
}
