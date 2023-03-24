using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.KeyboardBacklight.Spectrum;
using Microsoft.Win32;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum;

public partial class SpectrumKeyboardBacklightControl
{
    private readonly TimeSpan _refreshStateInterval = TimeSpan.FromMilliseconds(50);

    private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();
    private readonly SpecialKeyListener _listener = IoCContainer.Resolve<SpecialKeyListener>();
    private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();
    private readonly SpectrumKeyboardSettings _settings = IoCContainer.Resolve<SpectrumKeyboardSettings>();

    private CancellationTokenSource? _refreshStateCancellationTokenSource;
    private Task? _refreshStateTask;

    private RadioButton[] ProfileButtons => new[]
    {
        _profileButton1,
        _profileButton2,
        _profileButton3,
        _profileButton4,
        _profileButton5,
        _profileButton6
    };

    protected override bool DisablesWhileRefreshing => false;

    public SpectrumKeyboardBacklightControl()
    {
        InitializeComponent();

        IsVisibleChanged += SpectrumKeyboardBacklightControl_IsVisibleChanged;
        SizeChanged += SpectrumKeyboardBacklightControl_SizeChanged;

        _listener.Changed += Listener_Changed;
    }

    private async void SpectrumKeyboardBacklightControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!IsVisible)
        {
            await StopAnimationAsync();
            _effects.Children.Clear();
            return;
        }

        await StartAnimationAsync();
    }

    private void SpectrumKeyboardBacklightControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_device.LayoutTransform is not ScaleTransform scaleTransform)
            return;

        var target = 0.75 * ActualWidth / _device.ActualWidth;
        var scale = Math.Clamp(target, 0.5, 1.5);

        scaleTransform.ScaleX = scale;
        scaleTransform.ScaleY = scale;
    }

    private void Listener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
    {
        if (!IsLoaded || !IsVisible)
            return;

        if (!await _controller.IsSupportedAsync().ConfigureAwait(false))
            return;

        if (await _vantage.GetStatusAsync() == SoftwareStatus.Enabled)
            return;

        switch (e)
        {
            case SpecialKey.SpectrumBacklightOff
                or SpecialKey.SpectrumBacklight1
                or SpecialKey.SpectrumBacklight2
                or SpecialKey.SpectrumBacklight3:
                await RefreshBrightnessAsync();
                break;
            case SpecialKey.SpectrumPreset1
                or SpecialKey.SpectrumPreset2
                or SpecialKey.SpectrumPreset3
                or SpecialKey.SpectrumPreset4
                or SpecialKey.SpectrumPreset5
                or SpecialKey.SpectrumPreset6:
                await RefreshProfileAsync();
                break;
        }
    });

    private async void BrightnessSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var value = (int)_brightnessSlider.Value;
        if (await _controller.GetBrightnessAsync() != value)
            await _controller.SetBrightnessAsync(value);
    }

    private async void ProfileButton_OnClick(object sender, RoutedEventArgs e)
    {
        await StopAnimationAsync();

        if ((sender as RadioButton)?.Tag is not int profile)
            return;

        _brightnessSlider.IsEnabled = false;
        foreach (var profileButton in ProfileButtons)
            profileButton.IsEnabled = false;

        if (await _controller.GetProfileAsync() != profile)
        {
            await _controller.SetProfileAsync(profile);
            await RefreshProfileDescriptionAsync();
        }

        foreach (var profileButton in ProfileButtons)
            profileButton.IsEnabled = true;
        _brightnessSlider.IsEnabled = true;

        if (IsVisible)
            await StartAnimationAsync();
    }

    private void SelectableControl_Selected(object? sender, SelectableControl.SelectedEventArgs e)
    {
        foreach (var button in _device.GetVisibleButtons().Where(b => !(b.IsChecked ?? false)))
            button.IsChecked = e.ContainsCenter(button);
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e) => SelectAllButtons();

    private void DeselectAll_Click(object sender, RoutedEventArgs e) => DeselectAllButtons();

    private async void SwitchKeyboardLayout_Click(object sender, RoutedEventArgs e)
    {
        await StopAnimationAsync();

        var buttons = _device.GetVisibleButtons();
        foreach (var button in buttons)
            button.IsChecked = false;

        var currentKeyboardLayout = _settings.Store.KeyboardLayout;
        var keyboardLayout = currentKeyboardLayout switch
        {
            KeyboardLayout.Ansi => KeyboardLayout.Iso,
            KeyboardLayout.Iso => KeyboardLayout.Ansi,
            _ => throw new ArgumentException(nameof(currentKeyboardLayout))
        };

        _settings.Store.KeyboardLayout = keyboardLayout;
        _settings.SynchronizeStore();

        var (spectrumLayout, _, keys) = await _controller.GetKeyboardLayoutAsync();

        _device.SetLayout(spectrumLayout, keyboardLayout, keys);

        if (IsVisible)
            await StartAnimationAsync();
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sfd = new SaveFileDialog
            {
                Title = Resource.Export,
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                Filter = "Json Files (.json)|*.json",
            };

            var result = sfd.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var profile = await _controller.GetProfileAsync();
            await _controller.ExportProfileDescriptionAsync(profile, sfd.FileName);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't export profile.", ex);

            await SnackbarHelper.ShowAsync(Resource.SpectrumKeyboardBacklightControl_ExportProfileError_Title, Resource.SpectrumKeyboardBacklightControl_ExportProfileError_Message, true);
        }
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var ofd = new OpenFileDialog
            {
                Title = Resource.Import,
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                Filter = "Json Files (.json)|*.json",
                CheckFileExists = true,
            };
            var result = ofd.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var profile = await _controller.GetProfileAsync();
            await _controller.ImportProfileDescription(profile, ofd.FileName);

            await RefreshProfileDescriptionAsync();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't import profile.", ex);

            await SnackbarHelper.ShowAsync(Resource.SpectrumKeyboardBacklightControl_ImportProfileError_Title, Resource.SpectrumKeyboardBacklightControl_ImportProfileError_Message, true);
        }
    }

    private void AddEffectButton_Click(object sender, RoutedEventArgs e)
    {
        var buttons = _device.GetVisibleButtons().ToArray();
        var checkedButtons = buttons.Where(b => b.IsChecked ?? false).ToArray();

        if (checkedButtons.IsEmpty())
        {
            SelectAllButtons();
            checkedButtons = buttons;
        }

        var keyCodes = checkedButtons.Select(b => b.KeyCode).ToArray();

        CreateEffect(keyCodes);
    }

    private async void ResetToDefaultButton_Click(object sender, RoutedEventArgs e) => await ResetToDefaultAsync();

    protected override async Task OnRefreshAsync()
    {
        if (!await _controller.IsSupportedAsync())
            throw new InvalidOperationException("Spectrum Keyboard does not seem to be supported");

        var vantageStatus = await _vantage.GetStatusAsync();
        if (vantageStatus == SoftwareStatus.Enabled)
        {
            _vantageWarningCard.Visibility = Visibility.Visible;
            _content.IsEnabled = false;
            _noEffectsText.Visibility = Visibility.Collapsed;
            return;
        }

        _vantageWarningCard.Visibility = Visibility.Collapsed;

        var (spectrumLayout, keyboardLayout, keys) = await _controller.GetKeyboardLayoutAsync();

        if (!_settings.Store.KeyboardLayout.HasValue)
        {
            _settings.Store.KeyboardLayout = keyboardLayout;
            _settings.SynchronizeStore();
        }
        else
        {
            keyboardLayout = _settings.Store.KeyboardLayout.Value;
        }

        _device.SetLayout(spectrumLayout, keyboardLayout, keys);

        _content.IsEnabled = true;

        await RefreshBrightnessAsync();
        await RefreshProfileAsync();

        await StartAnimationAsync();
    }

    protected override void OnFinishedLoading() { }

    private void SelectButtons(SpectrumKeyboardBacklightKeys keys)
    {
        if (keys.All)
        {
            SelectAllButtons();
            return;
        }

        DeselectAllButtons();

        foreach (var button in _device.GetVisibleButtons())
        {
            if (!keys.KeyCodes.Contains(button.KeyCode))
                continue;

            button.IsChecked = true;
        }
    }

    private void SelectAllButtons()
    {
        foreach (var button in _device.GetVisibleButtons())
            button.IsChecked = true;
    }

    private void DeselectAllButtons()
    {
        foreach (var button in _device.GetVisibleButtons())
            button.IsChecked = false;
    }

    private async Task StartAnimationAsync()
    {
        await StopAnimationAsync();

        _refreshStateCancellationTokenSource?.Cancel();
        _refreshStateCancellationTokenSource = new();

        _refreshStateTask = RefreshStateAsync(_refreshStateCancellationTokenSource.Token);
    }

    private async Task StopAnimationAsync()
    {
        _refreshStateCancellationTokenSource?.Cancel();

        if (_refreshStateTask is not null)
            await _refreshStateTask;

        _refreshStateTask = null;
    }

    private async Task RefreshStateAsync(CancellationToken token)
    {
        var buttons = _device.GetVisibleButtons().ToArray();

        if (buttons.Length < 1)
            return;

        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                var delay = Task.Delay(_refreshStateInterval, token);
                var state = await Task.Run(_controller.GetStateAsync, token);

                foreach (var button in buttons)
                {
                    if (!state.TryGetValue(button.KeyCode, out var rgb))
                    {
                        button.Color = null;
                        continue;
                    }

                    if (rgb is { R: < 1, G: < 1, B: < 1 })
                    {
                        button.Color = null;
                        continue;
                    }

                    button.Color = Color.FromRgb(rgb.R, rgb.G, rgb.B);
                }

                await delay;
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to refresh state.", ex);
        }
        finally
        {
            foreach (var button in buttons)
                button._background.Background = null;
        }
    }

    private async Task RefreshBrightnessAsync()
    {
        _brightnessSlider.Value = await _controller.GetBrightnessAsync();
    }

    private async Task RefreshProfileAsync()
    {
        var profile = await _controller.GetProfileAsync();
        var profileButton = ProfileButtons.FirstOrDefault(pb => pb.Tag.Equals(profile));
        if (profileButton is null)
            return;

        profileButton.IsChecked = true;

        await RefreshProfileDescriptionAsync();
    }

    private async Task RefreshProfileDescriptionAsync()
    {
        var profile = await _controller.GetProfileAsync();
        var (_, effects) = await _controller.GetProfileDescriptionAsync(profile);

        DeleteAllEffects();

        foreach (var effect in effects)
        {
            var control = CreateEffectControl(effect);
            _effects.Children.Add(control);
        }

        _noEffectsText.Visibility = effects.IsEmpty() ? Visibility.Visible : Visibility.Collapsed;
    }

    private async Task ApplyProfileAsync()
    {
        var profile = await _controller.GetProfileAsync();
        var effects = _effects.Children.OfType<SpectrumKeyboardEffectControl>().Select(c => c.Effect).ToArray();

        try
        {
            await _controller.SetProfileDescriptionAsync(profile, effects);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't apply profile.", ex);

            await SnackbarHelper.ShowAsync(Resource.SpectrumKeyboardBacklightControl_ApplyProfileError_Title, Resource.SpectrumKeyboardBacklightControl_ApplyProfileError_Message, true);
        }

        await RefreshProfileDescriptionAsync();
    }

    private async Task ResetToDefaultAsync()
    {
        DeselectAllButtons();

        var profile = await _controller.GetProfileAsync();
        await _controller.SetProfileDefaultAsync(profile);

        await RefreshProfileDescriptionAsync();
    }

    private void CreateEffect(ushort[] keyCodes)
    {
        var window = new SpectrumKeyboardBacklightEditEffectWindow(keyCodes) { Owner = Window.GetWindow(this) };
        window.Apply += async (_, e) => await AddEffect(e);
        window.ShowDialog();
    }

    private SpectrumKeyboardEffectControl CreateEffectControl(SpectrumKeyboardBacklightEffect effect)
    {
        var control = new SpectrumKeyboardEffectControl(effect);
        control.Click += (_, _) => SelectButtons(effect.Keys);
        control.Edit += (_, _) => EditEffect(control);
        control.Delete += async (_, _) => await DeleteEffectAsync(control);
        return control;
    }

    private async Task AddEffect(SpectrumKeyboardBacklightEffect effect)
    {
        DeselectAllButtons();

        if (effect.Keys.All)
            DeleteAllEffects();
        else if (_effects.Children.OfType<SpectrumKeyboardEffectControl>().Any(c => c.Effect.Keys.All))
            DeleteAllEffects();

        var control = CreateEffectControl(effect);
        _effects.Children.Add(control);

        await ApplyProfileAsync();
    }

    private void EditEffect(SpectrumKeyboardEffectControl effectControl)
    {
        var keyCodes = _device.GetVisibleButtons().Select(b => b.KeyCode).ToArray();
        var window = new SpectrumKeyboardBacklightEditEffectWindow(effectControl.Effect, keyCodes) { Owner = Window.GetWindow(this) };
        window.Apply += async (_, e) => await ReplaceEffectAsync(effectControl, e);
        window.ShowDialog();
    }

    private async Task ReplaceEffectAsync(SpectrumKeyboardEffectControl effectControl, SpectrumKeyboardBacklightEffect effect)
    {
        DeselectAllButtons();

        var control = new SpectrumKeyboardEffectControl(effect);
        control.Click += (_, _) => SelectButtons(effect.Keys);
        control.Edit += (_, _) => EditEffect(control);
        control.Delete += async (_, _) => await DeleteEffectAsync(control);

        var index = _effects.Children.IndexOf(effectControl);
        if (index < 0)
        {
            _effects.Children.Add(control);
        }
        else
        {
            _effects.Children.RemoveAt(index);
            _effects.Children.Insert(index, control);
        }

        await ApplyProfileAsync();
    }

    private async Task DeleteEffectAsync(SpectrumKeyboardEffectControl effectControl)
    {
        _effects.Children.Remove(effectControl);

        await ApplyProfileAsync();
    }

    private void DeleteAllEffects() => _effects.Children.Clear();
}