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
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorText = e.Exception.ToString();
            Trace.TraceError(errorText);
            MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(-1);
        }
    }
}