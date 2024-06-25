using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Integrations;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.SoftwareDisabler;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.System.Management;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.CLI;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Settings;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class SettingsPage
{
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();
    private readonly IntegrationsSettings _integrationsSettings = IoCContainer.Resolve<IntegrationsSettings>();

    private readonly VantageDisabler _vantageDisabler = IoCContainer.Resolve<VantageDisabler>();
    private readonly LegionZoneDisabler _legionZoneDisabler = IoCContainer.Resolve<LegionZoneDisabler>();
    private readonly FnKeysDisabler _fnKeysDisabler = IoCContainer.Resolve<FnKeysDisabler>();
    private readonly PowerModeFeature _powerModeFeature = IoCContainer.Resolve<PowerModeFeature>();
    private readonly RGBKeyboardBacklightController _rgbKeyboardBacklightController = IoCContainer.Resolve<RGBKeyboardBacklightController>();
    private readonly ThemeManager _themeManager = IoCContainer.Resolve<ThemeManager>();
    private readonly HWiNFOIntegration _hwinfoIntegration = IoCContainer.Resolve<HWiNFOIntegration>();
    private readonly IpcServer _ipcServer = IoCContainer.Resolve<IpcServer>();

    private bool _isRefreshing;

    public SettingsPage()
    {
        InitializeComponent();

        IsVisibleChanged += SettingsPage_IsVisibleChanged;

        _themeManager.ThemeApplied += ThemeManager_ThemeApplied;
    }

    private async void SettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private void ThemeManager_ThemeApplied(object? sender, EventArgs e)
    {
        if (!_isRefreshing)
            UpdateAccentColorPicker();
    }

    private async Task RefreshAsync()
    {
        _isRefreshing = true;

        var loadingTask = Task.Delay(TimeSpan.FromMilliseconds(500));

        var languages = LocalizationHelper.Languages.OrderBy(LocalizationHelper.LanguageDisplayName, StringComparer.InvariantCultureIgnoreCase).ToArray();
        var language = await LocalizationHelper.GetLanguageAsync();
        if (languages.Length > 1)
        {
            _langComboBox.SetItems(languages, language, LocalizationHelper.LanguageDisplayName);
            _langComboBox.Visibility = Visibility.Visible;
        }
        else
        {
            _langCardControl.Visibility = Visibility.Collapsed;
        }

        _temperatureComboBox.SetItems(Enum.GetValues<TemperatureUnit>(), _settings.Store.TemperatureUnit, t => t switch
        {
            TemperatureUnit.C => Resource.Celsius,
            TemperatureUnit.F => Resource.Fahrenheit,
            _ => new ArgumentOutOfRangeException(nameof(t))
        });
        _themeComboBox.SetItems(Enum.GetValues<Theme>(), _settings.Store.Theme, t => t.GetDisplayName());

        UpdateAccentColorPicker();
        _accentColorSourceComboBox.SetItems(Enum.GetValues<AccentColorSource>(), _settings.Store.AccentColorSource, t => t.GetDisplayName());

        _autorunComboBox.SetItems(Enum.GetValues<AutorunState>(), Autorun.State, t => t.GetDisplayName());
        _minimizeToTrayToggle.IsChecked = _settings.Store.MinimizeToTray;
        _minimizeOnCloseToggle.IsChecked = _settings.Store.MinimizeOnClose;

        var vantageStatus = await _vantageDisabler.GetStatusAsync();
        _vantageCard.Visibility = vantageStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
        _vantageToggle.IsChecked = vantageStatus == SoftwareStatus.Disabled;

        var legionZoneStatus = await _legionZoneDisabler.GetStatusAsync();
        _legionZoneCard.Visibility = legionZoneStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
        _legionZoneToggle.IsChecked = legionZoneStatus == SoftwareStatus.Disabled;

        var fnKeysStatus = await _fnKeysDisabler.GetStatusAsync();
        _fnKeysCard.Visibility = fnKeysStatus != SoftwareStatus.NotFound ? Visibility.Visible : Visibility.Collapsed;
        _fnKeysToggle.IsChecked = fnKeysStatus == SoftwareStatus.Disabled;

        _smartFnLockComboBox.SetItems([ModifierKey.None, ModifierKey.Alt, ModifierKey.Alt | ModifierKey.Ctrl | ModifierKey.Shift],
            _settings.Store.SmartFnLockFlags,
            m => m is ModifierKey.None ? Resource.Off : m.GetFlagsDisplayName(ModifierKey.None));

        _smartKeySinglePressActionCard.Visibility = fnKeysStatus != SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        _smartKeyDoublePressActionCard.Visibility = fnKeysStatus != SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;

        _notificationsCard.Visibility = fnKeysStatus != SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        _excludeRefreshRatesCard.Visibility = fnKeysStatus != SoftwareStatus.Enabled ? Visibility.Visible : Visibility.Collapsed;
        _synchronizeBrightnessToAllPowerPlansToggle.IsChecked = _settings.Store.SynchronizeBrightnessToAllPowerPlans;
        _onBatterySinceResetToggle.IsChecked = _settings.Store.ResetBatteryOnSinceTimerOnReboot;

        _bootLogoCard.Visibility = await BootLogo.IsSupportedAsync() ? Visibility.Visible : Visibility.Collapsed;

        try
        {
            var mi = await Compatibility.GetMachineInformationAsync();
            if (mi.Features[CapabilityID.GodModeFnQSwitchable])
            {
                _godModeFnQSwitchableCard.Visibility = Visibility.Visible;
                _godModeFnQSwitchableToggle.IsChecked = await WMI.LenovoOtherMethod.GetFeatureValueAsync(CapabilityID.GodModeFnQSwitchable) == 1;
            }
            else
            {
                _godModeFnQSwitchableCard.Visibility = Visibility.Collapsed;
            }
        }
        catch (Exception ex)
        {
            _godModeFnQSwitchableCard.Visibility = Visibility.Collapsed;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to get GodModeFnQSwitchable status.", ex);
        }

        _powerModeMappingComboBox.SetItems(Enum.GetValues<PowerModeMappingMode>(), _settings.Store.PowerModeMappingMode, t => t.GetDisplayName());

        var isPowerModeFeatureSupported = await _powerModeFeature.IsSupportedAsync();
        _powerModeMappingCard.Visibility = isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
        _powerModesCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerMode && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
        _windowsPowerPlansCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerPlan && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
        _windowsPowerPlansControlPanelCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerPlan && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;

        _onBatterySinceResetToggle.Visibility = Visibility.Visible;

        _hwinfoIntegrationToggle.IsChecked = _integrationsSettings.Store.HWiNFO;
        _cliInterfaceToggle.IsChecked = _integrationsSettings.Store.CLI;
        _cliPathToggle.IsChecked = SystemPath.HasCLI();

        await loadingTask;

        _temperatureComboBox.Visibility = Visibility.Visible;
        _themeComboBox.Visibility = Visibility.Visible;
        _autorunComboBox.Visibility = Visibility.Visible;
        _minimizeToTrayToggle.Visibility = Visibility.Visible;
        _minimizeOnCloseToggle.Visibility = Visibility.Visible;
        _vantageToggle.Visibility = Visibility.Visible;
        _legionZoneToggle.Visibility = Visibility.Visible;
        _fnKeysToggle.Visibility = Visibility.Visible;
        _smartFnLockComboBox.Visibility = Visibility.Visible;
        _synchronizeBrightnessToAllPowerPlansToggle.Visibility = Visibility.Visible;
        _godModeFnQSwitchableToggle.Visibility = Visibility.Visible;
        _powerModeMappingComboBox.Visibility = Visibility.Visible;
        _hwinfoIntegrationToggle.Visibility = Visibility.Visible;
        _cliInterfaceToggle.Visibility = Visibility.Visible;
        _cliPathToggle.Visibility = Visibility.Visible;

        _isRefreshing = false;
    }

    private async void LangComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_langComboBox.TryGetSelectedItem(out CultureInfo? cultureInfo) || cultureInfo is null)
            return;

        await LocalizationHelper.SetLanguageAsync(cultureInfo);

        App.Current.RestartMainWindow();
    }

    private void TemperatureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_temperatureComboBox.TryGetSelectedItem(out TemperatureUnit temperatureUnit))
            return;

        _settings.Store.TemperatureUnit = temperatureUnit;
        _settings.SynchronizeStore();
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_themeComboBox.TryGetSelectedItem(out Theme state))
            return;

        _settings.Store.Theme = state;
        _settings.SynchronizeStore();

        _themeManager.Apply();
    }

    private void AccentColorPicker_Changed(object sender, EventArgs e)
    {
        if (_isRefreshing)
            return;

        if (_settings.Store.AccentColorSource != AccentColorSource.Custom)
            return;

        _settings.Store.AccentColor = _accentColorPicker.SelectedColor.ToRGBColor();
        _settings.SynchronizeStore();

        _themeManager.Apply();
    }

    private void AccentColorSourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_accentColorSourceComboBox.TryGetSelectedItem(out AccentColorSource state))
            return;

        _settings.Store.AccentColorSource = state;
        _settings.SynchronizeStore();

        UpdateAccentColorPicker();

        _themeManager.Apply();
    }

    private void UpdateAccentColorPicker()
    {
        _accentColorPicker.Visibility = _settings.Store.AccentColorSource == AccentColorSource.Custom ? Visibility.Visible : Visibility.Collapsed;
        _accentColorPicker.SelectedColor = _themeManager.GetAccentColor().ToColor();
    }

    private void AutorunComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_autorunComboBox.TryGetSelectedItem(out AutorunState state))
            return;

        Autorun.Set(state);
    }

    private void SmartFnLockComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_smartFnLockComboBox.TryGetSelectedItem(out ModifierKey modifierKey))
            return;

        _settings.Store.SmartFnLockFlags = modifierKey;
        _settings.SynchronizeStore();
    }

    private void SmartKeySinglePressActionCard_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new SelectSmartKeyPipelinesWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void SmartKeyDoublePressActionCard_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new SelectSmartKeyPipelinesWindow(isDoublePress: true) { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void MinimizeToTrayToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var state = _minimizeToTrayToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.MinimizeToTray = state.Value;
        _settings.SynchronizeStore();
    }

    private void MinimizeOnCloseToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var state = _minimizeOnCloseToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.MinimizeOnClose = state.Value;
        _settings.SynchronizeStore();
    }

    private async void VantageToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        _vantageToggle.IsEnabled = false;

        var state = _vantageToggle.IsChecked;
        if (state is null)
            return;

        if (state.Value)
        {
            try
            {
                await _vantageDisabler.DisableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_DisableVantage_Error_Title, Resource.SettingsPage_DisableVantage_Error_Message, SnackbarType.Error);
                return;
            }

            try
            {
                if (await _rgbKeyboardBacklightController.IsSupportedAsync())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Setting light control owner and restoring preset...");

                    await _rgbKeyboardBacklightController.SetLightControlOwnerAsync(true, true);
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set light control owner or current preset.", ex);
            }

            try
            {
                var controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();
                if (await controller.IsSupportedAsync())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Starting Aurora if needed...");

                    var result = await controller.StartAuroraIfNeededAsync();
                    if (result)
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Aurora started.");
                    }
                    else
                    {
                        if (Log.Instance.IsTraceEnabled)
                            Log.Instance.Trace($"Aurora not needed.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't start Aurora if needed.", ex);
            }
        }
        else
        {
            try
            {
                if (await _rgbKeyboardBacklightController.IsSupportedAsync())
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Setting light control owner...");

                    await _rgbKeyboardBacklightController.SetLightControlOwnerAsync(false);
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't set light control owner.", ex);
            }

            try
            {
                if (IoCContainer.TryResolve<SpectrumKeyboardBacklightController>() is { } spectrumKeyboardBacklightController)
                {
                    if (Log.Instance.IsTraceEnabled)
                        Log.Instance.Trace($"Making sure Aurora is stopped...");

                    if (await spectrumKeyboardBacklightController.IsSupportedAsync())
                        await spectrumKeyboardBacklightController.StopAuroraIfNeededAsync();
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't stop Aurora.", ex);
            }

            try
            {
                await _vantageDisabler.EnableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_EnableVantage_Error_Title, Resource.SettingsPage_EnableVantage_Error_Message, SnackbarType.Error);
                return;
            }
        }

        _vantageToggle.IsEnabled = true;
    }

    private async void LegionZoneToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        _legionZoneToggle.IsEnabled = false;

        var state = _legionZoneToggle.IsChecked;
        if (state is null)
            return;

        if (state.Value)
        {
            try
            {
                await _legionZoneDisabler.DisableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_DisableLegionZone_Error_Title, Resource.SettingsPage_DisableLegionZone_Error_Message, SnackbarType.Error);
                return;
            }
        }
        else
        {
            try
            {
                await _legionZoneDisabler.EnableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_EnableLegionZone_Error_Title, Resource.SettingsPage_EnableLegionZone_Error_Message, SnackbarType.Error);
                return;
            }
        }

        _legionZoneToggle.IsEnabled = true;
    }

    private async void FnKeysToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        _fnKeysToggle.IsEnabled = false;

        var state = _fnKeysToggle.IsChecked;
        if (state is null)
            return;

        if (state.Value)
        {
            try
            {
                await _fnKeysDisabler.DisableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_DisableLenovoHotkeys_Error_Title, Resource.SettingsPage_DisableLenovoHotkeys_Error_Message, SnackbarType.Error);
                return;
            }
        }
        else
        {
            try
            {
                await _fnKeysDisabler.EnableAsync();
            }
            catch
            {
                await SnackbarHelper.ShowAsync(Resource.SettingsPage_EnableLenovoHotkeys_Error_Title, Resource.SettingsPage_EnableLenovoHotkeys_Error_Message, SnackbarType.Error);
                return;
            }
        }

        _fnKeysToggle.IsEnabled = true;

        _smartKeySinglePressActionCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
        _smartKeyDoublePressActionCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
        _notificationsCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
        _excludeRefreshRatesCard.Visibility = state.Value ? Visibility.Visible : Visibility.Collapsed;
    }

    private void NotificationsCard_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new NotificationsSettingsWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void ExcludeRefreshRates_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new ExcludeRefreshRatesWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void SynchronizeBrightnessToAllPowerPlansToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var state = _synchronizeBrightnessToAllPowerPlansToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.SynchronizeBrightnessToAllPowerPlans = state.Value;
        _settings.SynchronizeStore();
    }

    private void BootLogo_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new BootLogoWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private async void GodModeFnQSwitchableToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var state = _godModeFnQSwitchableToggle.IsChecked;
        if (state is null)
            return;

        _godModeFnQSwitchableToggle.IsEnabled = false;

        await WMI.LenovoOtherMethod.SetFeatureValueAsync(CapabilityID.GodModeFnQSwitchable, state.Value ? 1 : 0);

        _godModeFnQSwitchableToggle.IsEnabled = true;
    }

    private async void PowerModeMappingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isRefreshing)
            return;

        if (!_powerModeMappingComboBox.TryGetSelectedItem(out PowerModeMappingMode powerModeMappingMode))
            return;

        _settings.Store.PowerModeMappingMode = powerModeMappingMode;
        _settings.SynchronizeStore();

        var isPowerModeFeatureSupported = await _powerModeFeature.IsSupportedAsync();
        _powerModesCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerMode && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
        _windowsPowerPlansCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerPlan && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
        _windowsPowerPlansControlPanelCard.Visibility = _settings.Store.PowerModeMappingMode == PowerModeMappingMode.WindowsPowerPlan && isPowerModeFeatureSupported ? Visibility.Visible : Visibility.Collapsed;
    }

    private void WindowsPowerPlans_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new WindowsPowerPlansWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void PowerModes_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var window = new WindowsPowerModesWindow { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private void WindowsPowerPlansControlPanel_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("control", "/name Microsoft.PowerOptions");
    }

    private void OnBatterySinceResetToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        var state = _onBatterySinceResetToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.ResetBatteryOnSinceTimerOnReboot = state.Value;
        _settings.SynchronizeStore();
    }

    private async void HWiNFOIntegrationToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        _integrationsSettings.Store.HWiNFO = _hwinfoIntegrationToggle.IsChecked ?? false;
        _integrationsSettings.SynchronizeStore();

        await _hwinfoIntegration.StartStopIfNeededAsync();
    }

    private async void CLIInterfaceToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        _integrationsSettings.Store.CLI = _cliInterfaceToggle.IsChecked ?? false;
        _integrationsSettings.SynchronizeStore();

        await _ipcServer.StartStopIfNeededAsync();
    }

    private void CLIPathToggle_Click(object sender, RoutedEventArgs e)
    {
        if (_isRefreshing)
            return;

        SystemPath.SetCLI(_cliPathToggle.IsChecked ?? false);
    }
}
