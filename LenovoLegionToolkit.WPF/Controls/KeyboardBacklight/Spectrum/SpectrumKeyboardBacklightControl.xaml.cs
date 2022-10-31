using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardBacklightControl
    {
        private readonly TimeSpan _refreshStateInterval = TimeSpan.FromMilliseconds(100);

        private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();
        private readonly SpecialKeyListener _listener = IoCContainer.Resolve<SpecialKeyListener>();
        private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();

        private CancellationTokenSource? _refreshStateCancellationTokenSource;

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

        private void SpectrumKeyboardBacklightControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
            {
                _refreshStateCancellationTokenSource?.Cancel();
                return;
            }

            _refreshStateCancellationTokenSource?.Cancel();
            _refreshStateCancellationTokenSource = new();

            _ = RefreshStateAsync(_refreshStateCancellationTokenSource.Token);
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

        private async Task RefreshStateAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var delay = Task.Delay(_refreshStateInterval, token);

                    var state = await _controller.GetStateAsync();

                    foreach (var button in _device.Buttons)
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

                    if (Log.Instance.IsTraceEnabled && _device.Buttons.Length != state.Count)
                    {
                        var codes = state.Keys.Except(_device.Buttons.Select(b => b.KeyCode)).Select(kc => $"{kc:X}");
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
                foreach (var button in _device.Buttons)
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

            if (await _controller.GetProfileAsync() != profile)
                await _controller.SetProfileAsync(profile);
        }
    }
}
