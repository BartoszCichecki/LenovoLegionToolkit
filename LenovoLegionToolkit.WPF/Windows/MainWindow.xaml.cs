using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Pages;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Controls;

#pragma warning disable IDE0052 // Remove unread private members

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class MainWindow
    {
        private readonly TimeSpan _fnF9DoublePressInterval = TimeSpan.FromMilliseconds(500);

        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
        private readonly FnKeys _fnKeys = IoCContainer.Resolve<FnKeys>();
        private readonly SpecialKeyListener _specialKeyListener = IoCContainer.Resolve<SpecialKeyListener>();
        private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
        private readonly UpdateChecker _updateChecker = IoCContainer.Resolve<UpdateChecker>();

        public bool SuppressClosingEventHandler { get; set; }

        public Snackbar Snackbar => _snackbar;

        private SystemEventInterceptor? _systemEventInterceptor;
        private NotifyIcon? _notifyIcon;

        private DateTime _lastFnF9Press = DateTime.MinValue;
        private CancellationTokenSource? _fnF9DoublePressCancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += MainWindow_SourceInitialized;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
            IsVisibleChanged += MainWindow_IsVisibleChanged;
            StateChanged += MainWindow_StateChanged;

            if (Assembly.GetEntryAssembly()?.GetName()?.Version == new Version(0, 0, 1, 0))
                _title.Text += " [BETA]";

#if DEBUG
            _title.Text += " [DEBUG]";
#endif

            if (Log.Instance.IsTraceEnabled)
            {
                _title.Text += " [LOGGING ENABLED]";
                _openLogIndicator.Visibility = Visibility.Visible;
            }

            _specialKeyListener.Changed += SpecialKeyListener_Changed;
        }

        private void InitializeTray()
        {
            _notifyIcon?.Unregister();

            ContextMenuHelper.Instance.BringToForeground = BringToForeground;
            ContextMenuHelper.Instance.Close = App.Current.ShutdownAsync;

            var notifyIcon = new NotifyIcon
            {
                TooltipText = Resource.AboutPage_AppName,
                Icon = ImageSourceExtensions.ApplicationIcon(),
                FocusOnLeftClick = false,
                MenuOnRightClick = true,
                Menu = ContextMenuHelper.Instance.ContextMenu,
            };
            notifyIcon.LeftClick += NotifyIcon_LeftClick;
            notifyIcon.Register();

            _notifyIcon = notifyIcon;
        }

        private void SpecialKeyListener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            if (e != SpecialKey.Fn_F9)
                return;

            if (await _fnKeys.GetStatusAsync() == SoftwareStatus.Enabled)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Ignoring Fn+F9 FnKeys are enabled.");

                return;
            }

            _fnF9DoublePressCancellationTokenSource?.Cancel();
            _fnF9DoublePressCancellationTokenSource = new CancellationTokenSource();

            var token = _fnF9DoublePressCancellationTokenSource.Token;

            _ = Task.Run(async () =>
            {
                var now = DateTime.UtcNow;
                var diff = now - _lastFnF9Press;
                _lastFnF9Press = now;

                if (diff < _fnF9DoublePressInterval)
                {
                    await ProcessSpecialKey(isDoublePress: true);
                }
                else
                {
                    await Task.Delay(_fnF9DoublePressInterval, token);

                    await ProcessSpecialKey(isDoublePress: false);
                }
            }, token);
        });

        private async Task ProcessSpecialKey(bool isDoublePress)
        {
            var currentGuid = isDoublePress ? _settings.Store.SmartKeyDoublePressActionId : _settings.Store.SmartKeySinglePressActionId;

            // When currentGuid == null -> Show app (keeps the old behaviour)
            //                  == Guid.Empty -> Smart key is disabled
            //                  == pipeline guid -> Try to locate it in the list and process

            if (currentGuid == Guid.Empty)
                return;

            if (currentGuid == null)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Bringing to foreground after {(isDoublePress ? "double" : "single")} Fn+F9 press.");

                Dispatcher.Invoke(BringToForeground);
                return;
            }

            var guids = isDoublePress ? _settings.Store.SmartKeyDoublePressActionList : _settings.Store.SmartKeySinglePressActionList;

            if (guids.IsEmpty())
                guids.Add(currentGuid.Value);

            int currentIndex = guids.IndexOf(currentGuid.Value);
            if (currentIndex < 0)
                currentIndex = 0;

            int nextIndex = (currentIndex + 1) % guids.Count;

            var id = guids[currentIndex];

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Running action {id} after {(isDoublePress ? "double" : "single")} Fn+F9 press.");

            try
            {
                var pipeline = (await _automationProcessor.GetPipelinesAsync()).FirstOrDefault(p => p.Id == id);
                if (pipeline != null)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Running action {id} after {(isDoublePress ? "double" : "single")} Fn+F9 press.");

                    await _automationProcessor.RunNowAsync(pipeline.Id);

                    MessagingCenter.Publish(new Notification(isDoublePress ? NotificationType.SmartKeyDoublePress : NotificationType.SmartKeySinglePress,
                        NotificationDuration.Short, pipeline.Name ?? string.Empty));
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Running action {id} after {(isDoublePress ? "double" : "single")} Fn+F9 press failed.", ex);
            }

            if (isDoublePress)
                _settings.Store.SmartKeyDoublePressActionId = guids[nextIndex];
            else
                _settings.Store.SmartKeySinglePressActionId = guids[nextIndex];

            _settings.SynchronizeStore();
        }

        private void MainWindow_SourceInitialized(object? sender, EventArgs args)
        {
            var systemEventInterceptor = new SystemEventInterceptor(this);
            systemEventInterceptor.OnTaskbarCreated += (_, _) => InitializeTray();
            systemEventInterceptor.OnDisplayDeviceArrival += (_, _) => Task.Run(IoCContainer.Resolve<IGPUModeFeature>().NotifyAsync);
            systemEventInterceptor.OnResumed += (_, _) => Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await IoCContainer.Resolve<IGPUModeFeature>().NotifyAsync().ConfigureAwait(false);
            });

            _systemEventInterceptor = systemEventInterceptor;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(500);

            if (!await KeyboardBacklightPage.IsSupportedAsync())
                _navigationStore.Items.Remove(_keyboardItem);

            ContextMenuHelper.Instance.SetNavigationItems(_navigationStore);

            await loadingTask;

            _loader.IsLoading = false;

            LoadDeviceInfo();
            CheckForUpdates();

            InitializeTray();
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (SuppressClosingEventHandler)
                return;

            if (_settings.Store.MinimizeOnClose)
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

                _notifyIcon?.Unregister();

                await App.Current.ShutdownAsync();
            }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _systemEventInterceptor = null;
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Window state changed to {WindowState}");

            switch (WindowState)
            {
                case WindowState.Minimized:
                    SendToTray();
                    break;
                case WindowState.Normal:
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

        private void OpenLogIndicator_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                new Uri(Log.Instance.LogPath).Open();
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Failed to open log.", ex);
            }
        }

        private void DeviceInfoIndicator_Click(object sender, RoutedEventArgs e)
        {
            var deviceInformationWindow = new DeviceInformationWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            deviceInformationWindow.ShowDialog();
        }

        private void UpdateIndicator_Click(object sender, RoutedEventArgs e)
        {
            var updateWindow = new UpdateWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            updateWindow.ShowDialog();
        }

        private void NotifyIcon_LeftClick([NotNull] NotifyIcon sender, RoutedEventArgs e) => BringToForeground();

        private void LoadDeviceInfo()
        {
            Task.Run(Compatibility.GetMachineInformationAsync)
                .ContinueWith(mi =>
                {
                    _deviceInfoIndicator.Content = mi.Result.Model;
                    _deviceInfoIndicator.Visibility = Visibility.Visible;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CheckForUpdates()
        {
            Task.Run(_updateChecker.Check)
                .ContinueWith(updatesAvailable =>
                {
                    var result = updatesAvailable.Result;
                    if (result is null)
                    {
                        _updateIndicator.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _updateIndicator.Content = string.Format(Resource.MainWindow_UpdateAvailableWithVersion, result.ToString(3));
                        _updateIndicator.Visibility = Visibility.Visible;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void BringToForeground()
        {
            ShowInTaskbar = true;

            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }

        public void SendToTray()
        {
            Hide();
            ShowInTaskbar = false;
        }
    }
}
