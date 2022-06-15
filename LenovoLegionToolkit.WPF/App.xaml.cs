using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit
{
    public partial class App : Application
    {
        private const string MutexName = "LenovoLegionToolkit_Mutex_6efcc882-924c-4cbc-8fec-f45c25696f98";
        private const string EventName = "LenovoLegionToolkit_Event_6efcc882-924c-4cbc-8fec-f45c25696f98";

        private Mutex? _mutex;
        private EventWaitHandle? _eventWaitHandle;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (IsTraceEnabled(e.Args))
                Log.Instance.IsTraceEnabled = true;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [version={Assembly.GetEntryAssembly()?.GetName().Version}]");

            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            EnsureSingleInstance();

            if (!ShouldByPassCompatibilityCheck(e.Args))
                await CheckCompatibilityAsync();

            IoCContainer.Initialize(
                new Lib.IoCModule(),
                new Lib.Automation.IoCModule(),
                new WPF.IoCModule()
            );

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Initializing automation processor...");

                var automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
                await automationProcessor.InitializeAsync();
                automationProcessor.RunOnStartup();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't initialize automation processor. Exception: {ex.Demystify()}");
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ensuring correct power plan is set...");

                await IoCContainer.Resolve<PowerModeFeature>().EnsureCorrectPowerPlanIsSetAsync();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't ensure correct power plan. Exception: {ex.Demystify()}");
            }

            Autorun.Validate();

            IoCContainer.Resolve<ThemeManager>().Apply();

            var mainWindow = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            MainWindow = mainWindow;

            if (ShouldStartMinimized(e.Args))
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Sending MainWindow to tray...");
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.Show();
                mainWindow.SendToTray();
            }
            else
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Showing MainWindow...");
                mainWindow.Show();
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Start up complete");
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Instance.ErrorReport(e.Exception.Demystify());

            var errorText = e.Exception.Demystify().ToString();

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"***** *** Unhandled exception ***** ***\n{errorText}");

            MessageBox.Show(errorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }

        private async Task CheckCompatibilityAsync()
        {
            var (isCompatible, mi) = await Compatibility.IsCompatibleAsync();
            if (isCompatible)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Compatibility check passed");
                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Incompatible system detected, shutting down... [Vendor={mi.Vendor}, Model={mi.Model}]");

            MessageBox.Show($"This application is not compatible with:\n\n{mi.Vendor} {mi.Model}.", "Unsupported device", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }

        private void EnsureSingleInstance()
        {
            _mutex = new Mutex(true, MutexName, out bool isOwned);
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

        private static bool ShouldByPassCompatibilityCheck(string[] args)
        {
            var result = args.Contains("--skip-compat-check");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        private static bool ShouldStartMinimized(string[] args)
        {
            var result = args.Contains("--minimized");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        private static bool IsTraceEnabled(string[] args)
        {
            var result = args.Contains("--trace");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        #endregion

    }
}