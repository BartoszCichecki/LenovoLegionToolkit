using LenovoLegionToolkit.Lib.Utils;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace LenovoLegionToolkit
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (ShouldByPassCompatibilityCheck(e.Args))
                return;

            CheckCompatibility();
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
            var mi = OS.GetMachineInformation();
            if (Compatibility.IsCompatible(mi))
                return;

            MessageBox.Show($"This application is not compatible with:\n\n{mi.Vendor} {mi.Model}.", "Unsupported device", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }

        private static bool ShouldByPassCompatibilityCheck(string[] args)
        {
            return args.Length > 0 && args[0] == "--no-compat-check";
        }
    }
}