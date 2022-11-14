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
using LenovoLegionToolkit.WPF.Windows.KeyboardBacklight.Spectrum;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
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

            Loaded += SpectrumKeyboardBacklightControl_Loaded;
            IsVisibleChanged += SpectrumKeyboardBacklightControl_IsVisibleChanged;
            SizeChanged += SpectrumKeyboardBacklightControl_SizeChanged;

            _listener.Changed += Listener_Changed;
        }

        private void SpectrumKeyboardBacklightControl_Loaded(object sender, RoutedEventArgs e)
        {
            _device.SetLayout(_settings.Store.KeyboardLayout, _controller.IsExtendedSupported());
        }

        private async void SpectrumKeyboardBacklightControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                await StartAnimationAsync();
            else
                await StopAnimationAsync();
        }

        private void SpectrumKeyboardBacklightControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_device.LayoutTransform is not ScaleTransform scaleTransform)
                return;

            var target = 0.75 * ActualWidth / _device.ActualWidth;
            var scale = Math.Clamp(target, 0.5, 2);

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
        }

        private void Listener_Changed(object? sender, SpecialKey e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            if (!_controller.IsSupported() || await _vantage.GetStatusAsync() == SoftwareStatus.Enabled)
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

        protected override async Task OnRefreshAsync()
        {
            if (!_controller.IsSupported())
                throw new InvalidOperationException("Spectrum Keyboard does not seem to be supported");

            var vantageStatus = await _vantage.GetStatusAsync();
            if (vantageStatus == SoftwareStatus.Enabled)
            {
                _vantageWarningCard.Visibility = Visibility.Visible;
                _content.IsEnabled = false;

                return;
            }

            _vantageWarningCard.Visibility = Visibility.Collapsed;
            _content.IsEnabled = true;

            await RefreshBrightnessAsync();
            await RefreshProfileAsync();
        }

        protected override void OnFinishedLoading() { }

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

            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var delay = Task.Delay(_refreshStateInterval, token);

                    var state = await _controller.GetStateAsync();

                    foreach (var button in buttons)
                    {
                        if (!state.TryGetValue(button.KeyCode, out var rgb))
                        {
                            button.Color = null;
                            continue;
                        }

                        if (rgb.R < 1 && rgb.G < 1 && rgb.B < 1)
                        {
                            button.Color = null;
                            continue;
                        }

                        button.Color = Color.FromRgb(rgb.R, rgb.G, rgb.B);
                    }

                    if (Log.Instance.IsTraceEnabled && buttons.Length != state.Count)
                    {
                        var codes = state.Keys.Except(buttons.Select(b => b.KeyCode)).Select(kc => $"{kc:X}");
                        Log.Instance.Trace($"Some reported keycodes were not used: {string.Join(",", codes)}");
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

        private void ShowProfileDescriptionLoader()
        {
            _effectsLoader.IsLoading = true;
        }

        private async Task RefreshProfileDescriptionAsync()
        {
            _effectsLoader.IsLoading = true;

            var delay = Task.Delay(TimeSpan.FromMilliseconds(250));

            _effects.Children.Clear();

            var profile = await _controller.GetProfileAsync();
            var (_, description) = await _controller.GetProfileDescriptionAsync(profile);

            foreach (var effect in description.Effects)
                AddEffect(effect);

            await delay;

            _effectsLoader.IsLoading = false;
        }

        private async Task ApplyProfileAsync()
        {
            var profile = await _controller.GetProfileAsync();
            var effects = _effects.Children.OfType<SpectrumKeyboardEffectControl>().Select(c => c.Effect).ToArray();

            await _controller.SetProfileDescriptionAsync(profile, new(effects));
            await RefreshProfileDescriptionAsync();
        }

        private void AddEffect(SpectrumKeyboardBacklightEffect effect)
        {
            var control = new SpectrumKeyboardEffectControl(effect);
            control.Delete += async (s, e) =>
            {
                ShowProfileDescriptionLoader();

                _effects.Children.Remove(control);

                await ApplyProfileAsync();
            };
            _effects.Children.Add(control);
        }

        private async void BrightnessSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = (int)_brightnessSlider.Value;
            if (await _controller.GetBrightnessAsync() != value)
                await _controller.SetBrightnessAsync(value);
        }

        private async void ProfileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton)?.Tag is not int profile)
                return;

            _brightnessSlider.IsEnabled = false;
            foreach (var profileButton in ProfileButtons)
                profileButton.IsEnabled = false;

            if (await _controller.GetProfileAsync() != profile)
            {
                ShowProfileDescriptionLoader();

                await _controller.SetProfileAsync(profile);
                await RefreshProfileDescriptionAsync();
            }

            foreach (var profileButton in ProfileButtons)
                profileButton.IsEnabled = true;
            _brightnessSlider.IsEnabled = true;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            var buttons = _device.GetVisibleButtons();
            foreach (var button in buttons)
                button.IsChecked = true;
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            var buttons = _device.GetVisibleButtons();
            foreach (var button in buttons)
                button.IsChecked = false;
        }

        private async void SwitchKeyboardLayout_Click(object sender, RoutedEventArgs e)
        {
            await StopAnimationAsync();

            var buttons = _device.GetVisibleButtons();
            foreach (var button in buttons)
                button.IsChecked = false;

            var currentLayout = _settings.Store.KeyboardLayout;
            var layout = currentLayout switch
            {
                KeyboardLayout.Ansi => KeyboardLayout.Iso,
                KeyboardLayout.Iso => KeyboardLayout.Ansi,
                _ => throw new ArgumentException(nameof(currentLayout))
            };

            _settings.Store.KeyboardLayout = layout;
            _settings.SynchronizeStore();

            _device.SetLayout(layout, _controller.IsExtendedSupported());

            await StartAnimationAsync();
        }

        private void AddEffectButton_Click(object sender, RoutedEventArgs e)
        {
            var buttons = _device.GetVisibleButtons().ToArray();
            var checkedButtons = buttons.Where(b => b.IsChecked ?? false).ToArray();

            if (checkedButtons.IsEmpty())
            {
                foreach (var button in buttons)
                    button.IsChecked = true;

                checkedButtons = buttons;
            }

            var keyCodes = checkedButtons.Select(b => b.KeyCode).ToArray();

            var window = new SpectrumKeyboardBacklightEditEffectWindow(keyCodes)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            window.Apply += async (s, e) =>
            {
                ShowProfileDescriptionLoader();

                foreach (var button in buttons)
                    button.IsChecked = false;

                AddEffect(e);

                await ApplyProfileAsync();
                await RefreshProfileDescriptionAsync();
            };
            window.ShowDialog();
        }
    }
}
