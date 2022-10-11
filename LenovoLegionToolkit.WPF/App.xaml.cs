#if !DEBUG
using LenovoLegionToolkit.Lib.System;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows;
using LenovoLegionToolkit.WPF.Windows.Utils;
using WinFormsApp = System.Windows.Forms.Application;
using WinFormsHighDpiMode = System.Windows.Forms.HighDpiMode;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF
{
    public partial class App
    {
        private const string MutexName = "LenovoLegionToolkit_Mutex_6efcc882-924c-4cbc-8fec-f45c25696f98";
        private const string EventName = "LenovoLegionToolkit_Event_6efcc882-924c-4cbc-8fec-f45c25696f98";

        private Mutex? _mutex;
        private EventWaitHandle? _eventWaitHandle;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await CheckBasicCompatibilityAsync();

            var args = e.Args.Concat(LoadExternalArgs()).ToArray();

            if (IsTraceEnabled(args))
                Log.Instance.IsTraceEnabled = true;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [version={Assembly.GetEntryAssembly()?.GetName().Version}, build={Assembly.GetEntryAssembly()?.GetBuildDateTime()?.ToString("yyyyMMddHHmmss") ?? ""}]");

            WinFormsApp.SetHighDpiMode(WinFormsHighDpiMode.PerMonitorV2);
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            EnsureSingleInstance();

            if (!ShouldByPassCompatibilityCheck(args))
                await CheckCompatibilityAsync();

            IoCContainer.Initialize(
                new Lib.IoCModule(),
                new Lib.Automation.IoCModule(),
                new WPF.IoCModule()
            );

            if (ShouldForceDisableRGBKeyboardSupport(args))
                IoCContainer.Resolve<RGBKeyboardBacklightController>().ForceDisable = true;

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
                    Log.Instance.Trace($"Couldn't initialize automation processor.", ex);
            }

            try
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ensuring AI Mode is set...");

                await IoCContainer.Resolve<PowerModeFeature>().EnsureAIModeIsSetAsync();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set AI Mode.", ex);
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
                    Log.Instance.Trace($"Couldn't ensure correct power plan.", ex);
            }

            try
            {
                var rgbKeyboardBacklightController = IoCContainer.Resolve<RGBKeyboardBacklightController>();

                if (rgbKeyboardBacklightController.IsSupported())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Setting light control owner and restoring preset...");

                    await rgbKeyboardBacklightController.SetLightControlOwnerAsync(true, true);
                }
                else
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"RGB keyboard is not supported.");
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set light control owner or current preset.", ex);
            }

#if !DEBUG
            Autorun.Validate();
#endif

            var mainWindow = new MainWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            MainWindow = mainWindow;

            IoCContainer.Resolve<ThemeManager>().Apply();

            if (ShouldStartMinimized(args))
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

        public async Task ShutdownAsync()
        {
            try
            {
                if (IoCContainer.TryResolve<RGBKeyboardBacklightController>() is { } rgbKeyboardBacklightController)
                {
                    if (rgbKeyboardBacklightController.IsSupported())
                        await rgbKeyboardBacklightController.SetLightControlOwnerAsync(false);
                }
            }
            catch { }

            try
            {
                if (IoCContainer.TryResolve<PowerModeFeature>() is { } powerModeFeature)
                    await powerModeFeature.EnsureAIModeIsOffAsync();
            }
            catch { }

            Shutdown();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Instance.Trace($"Unhandled exception occurred.", e.Exception);
            Log.Instance.ErrorReport(e.Exception);

            MessageBox.Show($"Unexpected exception occurred:\n{e.Exception.Message}\n\nPlease report the issue on {Constants.BugReportUri}.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            Shutdown(1);
        }

        private async Task CheckBasicCompatibilityAsync()
        {
            var isCompatible = await Compatibility.CheckBasicCompatibility();
            if (isCompatible)
                return;

            MessageBox.Show("This device is not compatible with Lenovo Legion Toolkit.", "Lenovo Legion Toolkit", MessageBoxButton.OK, MessageBoxImage.Error);

            Shutdown(99);
        }

        private async Task CheckCompatibilityAsync()
        {
            var (isCompatible, mi) = await Compatibility.IsCompatibleAsync();
            if (isCompatible)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Compatibility check passed. [Vendor={mi.Vendor}, Model={mi.Model}, MachineType={mi.MachineType}, BIOS={mi.BIOSVersion}]");
                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Incompatible system detected. [Vendor={mi.Vendor}, Model={mi.Model}, MachineType={mi.MachineType}, BIOS={mi.BIOSVersion}]");

            var unsupportedWindow = new UnsupportedWindow(mi);
            unsupportedWindow.Show();

            var result = await unsupportedWindow.ShouldContinue;

            if (result)
            {
                Log.Instance.IsTraceEnabled = true;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Compatibility check OVERRIDE. [Vendor={mi.Vendor}, Model={mi.Model}, MachineType={mi.MachineType}, version={Assembly.GetEntryAssembly()?.GetName().Version}, build={Assembly.GetEntryAssembly()?.GetBuildDateTime()?.ToString("yyyyMMddHHmmss") ?? ""}]");
                return;
            }

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Shutting down... [Vendor={mi.Vendor}, Model={mi.Model}, MachineType={mi.MachineType}]");

            Shutdown(100);
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
                        Current.Dispatcher.BeginInvoke((() => ((MainWindow)Current.MainWindow).BringToForeground()));
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

        private static string[] LoadExternalArgs()
        {
            try
            {
                var argsFile = Path.Combine(Folders.AppData, "args.txt");
                if (!File.Exists(argsFile))
                    return Array.Empty<string>();
                return File.ReadAllLines(argsFile);
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        private static bool ShouldForceDisableRGBKeyboardSupport(IEnumerable<string> args)
        {
            var result = args.Contains("--force-disable-rgbkb");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        private static bool ShouldByPassCompatibilityCheck(IEnumerable<string> args)
        {
            var result = args.Contains("--skip-compat-check");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        private static bool ShouldStartMinimized(IEnumerable<string> args)
        {
            var result = args.Contains("--minimized");
            if (result)
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            return result;
        }

        private static bool IsTraceEnabled(IEnumerable<string> args)
        {
            var result = args.Contains("--trace");
            if (result)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Argument present");
            }

            if (!result)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"LeftShift+LeftCtrl down.");

                    result = true;
                }
            }

            return result;
        }

        #endregion

    }
}