using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class ApplicationExtensions
    {
        public static Continuable FreezeAndRestart(this Application application)
        {
            var semaphoreSlim = new SemaphoreSlim(1);

            var operation = application.Dispatcher.BeginInvoke(() =>
            {
                var mainWindow = application.MainWindow;
                mainWindow.Hide();
                var arguments = mainWindow.WindowState == WindowState.Minimized ? "--minimized" : "";

                semaphoreSlim.Wait();

                var currentProcess = Process.GetCurrentProcess();
                var location = currentProcess.MainModule!.FileName!;
                Process.Start(location, arguments);
                currentProcess.Kill();
            });

            return new(async () =>
            {
                semaphoreSlim.Release();
                await operation;
            });
        }
    }
}
