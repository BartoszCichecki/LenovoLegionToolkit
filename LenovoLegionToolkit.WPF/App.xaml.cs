using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string MutexName = "LenovoLegionToolkit_Mutex_6efcc882-924c-4cbc-8fec-f45c25696f98";
        private const string EventName = "LenovoLegionToolkit_Event_6efcc882-924c-4cbc-8fec-f45c25696f98";

        private EventWaitHandle _eventWaitHandle;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            EnsureSingleInstance();

            if (!ShouldByPassCompatibilityCheck(e.Args))
                CheckCompatibility();

            var mainWindow = new MainWindow();
            if (ShouldStartMinimized(e.Args))
                mainWindow.SendToTray();
            else
                mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorText = e.Exception.ToString();
            Trace.TraceError(errorText);
            MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
        }

        private void CheckCompatibility()
        {
            if (Compatibility.IsCompatible(out MachineInformation mi))
                return;

            MessageBox.Show($"This application is not compatible with:\n\n{mi.Vendor} {mi.Model}.", "Unsupported device", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }

        private void EnsureSingleInstance()
        {
            _ = new Mutex(true, MutexName, out bool isOwned);
            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, EventName);


            if (isOwned)
            {
                new Thread(() =>
                {
                    while (_eventWaitHandle.WaitOne())
                        Current.Dispatcher.BeginInvoke((Action)(() => ((MainWindow)Current.MainWindow).BringToForeground()));
                })
                {
                    IsBackground = true
                }.Start();
                return;
            }

            _eventWaitHandle.Set();
            Shutdown();
        }

        #region Arguments

        private static bool ShouldByPassCompatibilityCheck(string[] args) => args.Contains("--skip-compat-check");

        private static bool ShouldStartMinimized(string[] args) => args.Contains("--minimized");

        #endregion

    }
}