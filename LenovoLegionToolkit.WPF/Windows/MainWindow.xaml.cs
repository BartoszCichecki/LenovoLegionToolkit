using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Messaging;
using LenovoLegionToolkit.Lib.Messaging.Messages;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Pages;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Microsoft.Xaml.Behaviors.Core;
using Windows.Win32;
using Windows.Win32.System.Threading;
using Wpf.Ui.Controls;
#if !DEBUG
using System.Reflection;
using LenovoLegionToolkit.Lib.Extensions;
#endif

#pragma warning disable CA1416

namespace LenovoLegionToolkit.WPF.Windows;

public partial class MainWindow
{
    private readonly ApplicationSettings _applicationSettings = IoCContainer.Resolve<ApplicationSettings>();
    private readonly SpecialKeyListener _specialKeyListener = IoCContainer.Resolve<SpecialKeyListener>();
    private readonly VantageDisabler _vantageDisabler = IoCContainer.Resolve<VantageDisabler>();
    private readonly LegionZoneDisabler _legionZoneDisabler = IoCContainer.Resolve<LegionZoneDisabler>();
    private readonly FnKeysDisabler _fnKeysDisabler = IoCContainer.Resolve<FnKeysDisabler>();
    private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

    private TrayHelper? _trayHelper;

    public bool TrayTooltipEnabled { get; init; } = true;
    public bool DisableConflictingSoftwareWarning { get; set; }
    public bool SuppressClosingEventHandler { get; set; }

    public Snackbar Snackbar => _snackbar;

    public MainWindow()
    {
        InitializeComponent();

        Closing += MainWindow_Closing;
        Closed += MainWindow_Closed;
        IsVisibleChanged += MainWindow_IsVisibleChanged;
        Loaded += MainWindow_Loaded;
        SourceInitialized += MainWindow_SourceInitialized;
        StateChanged += MainWindow_StateChanged;

#if DEBUG
        _title.Text += Debugger.IsAttached ? " [DEBUGGER ATTACHED]" : " [DEBUG]";
#else
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        if (version is not null && version.IsBeta())
            _title.Text += " [BETA]";
#endif

        if (Log.Instance.IsTraceEnabled)
        {
            _title.Text += " [LOGGING ENABLED]";
            _openLogIndicator.Visibility = Visibility.Visible;
        }

        Title = _title.Text;
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e) => RestoreSize();

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _contentGrid.Visibility = Visibility.Hidden;

        if (!await KeyboardBacklightPage.IsSupportedAsync())
            _navigationStore.Items.Remove(_keyboardItem);

        SmartKeyHelper.Instance.BringToForeground = () => Dispatcher.Invoke(BringToForeground);

        _specialKeyListener.Changed += (_, args) =>
        {
            if (args.SpecialKey == SpecialKey.FnN)
                Dispatcher.Invoke(BringToForeground);
        };

        _contentGrid.Visibility = Visibility.Visible;

        LoadDeviceInfo();
        UpdateIndicators();
        CheckForUpdates();

        InputBindings.Add(new KeyBinding(new ActionCommand(_navigationStore.NavigateToNext), Key.Tab, ModifierKeys.Control));
        InputBindings.Add(new KeyBinding(new ActionCommand(_navigationStore.NavigateToPrevious), Key.Tab, ModifierKeys.Control | ModifierKeys.Shift));

        var key = (int)Key.D1;
        foreach (var item in _navigationStore.Items.OfType<NavigationItem>())
            InputBindings.Add(new KeyBinding(new ActionCommand(() => _navigationStore.Navigate(item.PageTag)), (Key)key++, ModifierKeys.Control));

        var trayHelper = new TrayHelper(_navigationStore, BringToForeground, TrayTooltipEnabled);
        await trayHelper.InitializeAsync();
        trayHelper.MakeVisible();
        _trayHelper = trayHelper;
    }

    private async void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        SaveSize();

        if (SuppressClosingEventHandler)
            return;

        if (_applicationSettings.Store.MinimizeOnClose)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Minimizing...");

            WindowState = WindowState.Minimized;
            e.Cancel = true;
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Closing...");

            await App.Current.ShutdownAsync();
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs args)
    {
        _trayHelper?.Dispose();
        _trayHelper = null;
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Window state changed to {WindowState}");

        switch (WindowState)
        {
            case WindowState.Minimized:
                SetEfficiencyMode(true);
                SendToTray();
                break;
            case WindowState.Normal:
                SetEfficiencyMode(false);
                BringToForeground();
                break;
        }
    }

    private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
            return;

        CheckForUpdates();
    }

    private void OpenLogIndicator_Click(object sender, MouseButtonEventArgs e) => OpenLog();

    private void OpenLogIndicator_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Enter and not Key.Space)
            return;

        OpenLog();
    }

    private void DeviceInfoIndicator_Click(object sender, MouseButtonEventArgs e) => ShowDeviceInfoWindow();

    private void DeviceInfoIndicator_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Enter and not Key.Space)
            return;

        ShowDeviceInfoWindow();
    }

    private void UpdateIndicator_Click(object sender, RoutedEventArgs e) => ShowUpdateWindow();

    private void UpdateIndicator_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key is not Key.Enter and not Key.Space)
            return;

        ShowUpdateWindow();
    }

    private void LoadDeviceInfo()
    {
        Task.Run(Compatibility.GetMachineInformationAsync)
            .ContinueWith(mi =>
            {
                _deviceInfoIndicator.Content = mi.Result.Model;
                _deviceInfoIndicator.Visibility = Visibility.Visible;
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void UpdateIndicators()
    {
        if (DisableConflictingSoftwareWarning)
            return;

        _vantageDisabler.OnRefreshed += (_, e) => Dispatcher.Invoke(() =>
        {
            _vantageIndicator.Visibility = e.Status == SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        });

        _legionZoneDisabler.OnRefreshed += (_, e) => Dispatcher.Invoke(() =>
        {
            _legionZoneIndicator.Visibility = e.Status == SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        });

        _fnKeysDisabler.OnRefreshed += (_, e) => Dispatcher.Invoke(() =>
        {
            _fnKeysIndicator.Visibility = e.Status == SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        });

        Task.Run(async () =>
        {
            _ = await _vantageDisabler.GetStatusAsync().ConfigureAwait(false);
            _ = await _legionZoneDisabler.GetStatusAsync().ConfigureAwait(false);
            _ = await _fnKeysDisabler.GetStatusAsync().ConfigureAwait(false);
        });
    }

    public void CheckForUpdates(bool manualCheck = false)
    {
        Task.Run(() => _updateChecker.CheckAsync(manualCheck))
            .ContinueWith(async updatesAvailable =>
            {
                var result = updatesAvailable.Result;
                if (result is null)
                {
                    _updateIndicator.Visibility = Visibility.Collapsed;

                    if (manualCheck && WindowState != WindowState.Minimized)
                    {
                        switch (_updateChecker.Status)
                        {
                            case UpdateCheckStatus.Success:
                                await SnackbarHelper.ShowAsync(Resource.MainWindow_CheckForUpdates_Success_Title);
                                break;
                            case UpdateCheckStatus.RateLimitReached:
                                await SnackbarHelper.ShowAsync(Resource.MainWindow_CheckForUpdates_Error_Title, Resource.MainWindow_CheckForUpdates_Error_ReachedRateLimit_Message, SnackbarType.Error);
                                break;
                            case UpdateCheckStatus.Error:
                                await SnackbarHelper.ShowAsync(Resource.MainWindow_CheckForUpdates_Error_Title, Resource.MainWindow_CheckForUpdates_Error_Unknown_Message, SnackbarType.Error);
                                break;
                        }
                    }
                }
                else
                {
                    var versionNumber = result.ToString(3);

                    _updateIndicatorText.Text =
                        string.Format(Resource.MainWindow_UpdateAvailableWithVersion, versionNumber);
                    _updateIndicator.Visibility = Visibility.Visible;

                    if (WindowState == WindowState.Minimized)
                        MessagingCenter.Publish(new NotificationMessage(NotificationType.UpdateAvailable, versionNumber));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void RestoreSize()
    {
        if (!_applicationSettings.Store.WindowSize.HasValue)
            return;

        Width = Math.Max(MinWidth, _applicationSettings.Store.WindowSize.Value.Width);
        Height = Math.Max(MinHeight, _applicationSettings.Store.WindowSize.Value.Height);

        var desktopWorkingArea = ScreenHelper.GetPrimaryDesktopWorkingArea();

        Left = (desktopWorkingArea.Width - Width) / 2 + desktopWorkingArea.Left;
        Top = (desktopWorkingArea.Height - Height) / 2 + desktopWorkingArea.Top;
    }

    private void SaveSize()
    {
        _applicationSettings.Store.WindowSize = WindowState != WindowState.Normal
            ? new(RestoreBounds.Width, RestoreBounds.Height)
            : new(Width, Height);
        _applicationSettings.SynchronizeStore();
    }

    private void BringToForeground() => WindowExtensions.BringToForeground(this);

    private static void OpenLog()
    {
        try
        {
            if (!Directory.Exists(Folders.AppData))
                return;

            Process.Start("explorer", Log.Instance.LogPath);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to open log.", ex);
        }
    }

    private void ShowDeviceInfoWindow()
    {
        var window = new DeviceInformationWindow { Owner = this };
        window.ShowDialog();
    }

    public void ShowUpdateWindow()
    {
        var window = new UpdateWindow { Owner = this };
        window.ShowDialog();
    }

    public void SendToTray()
    {
        if (!_applicationSettings.Store.MinimizeToTray)
            return;

        SetEfficiencyMode(true);
        Hide();
        ShowInTaskbar = true;
    }

    private static unsafe void SetEfficiencyMode(bool enabled)
    {
        var ptr = IntPtr.Zero;

        try
        {
            var priorityClass = enabled
                ? PROCESS_CREATION_FLAGS.IDLE_PRIORITY_CLASS
                : PROCESS_CREATION_FLAGS.NORMAL_PRIORITY_CLASS;
            PInvoke.SetPriorityClass(PInvoke.GetCurrentProcess(), priorityClass);

            var state = new PROCESS_POWER_THROTTLING_STATE
            {
                Version = PInvoke.PROCESS_POWER_THROTTLING_CURRENT_VERSION,
                ControlMask = PInvoke.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
                StateMask = enabled ? PInvoke.PROCESS_POWER_THROTTLING_EXECUTION_SPEED : 0,
            };

            var size = Marshal.SizeOf<PROCESS_POWER_THROTTLING_STATE>();
            ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(state, ptr, false);

            PInvoke.SetProcessInformation(PInvoke.GetCurrentProcess(),
                PROCESS_INFORMATION_CLASS.ProcessPowerThrottling,
                ptr.ToPointer(),
                (uint)size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
