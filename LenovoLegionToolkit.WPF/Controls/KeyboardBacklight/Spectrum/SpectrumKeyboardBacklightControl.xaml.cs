using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardBacklightControl
    {
        private readonly SpectrumKeyboardBacklightController _controller = IoCContainer.Resolve<SpectrumKeyboardBacklightController>();
        private readonly SpecialKeyListener _listener = IoCContainer.Resolve<SpecialKeyListener>();
        private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();

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

            SizeChanged += SpectrumKeyboardBacklightControl_SizeChanged;

            _listener.Changed += Listener_Changed;
        }

        private void SpectrumKeyboardBacklightControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (_device.LayoutTransform is not ScaleTransform scaleTransform)
                return;

            var target = (0.75 * ActualWidth) / _device.ActualWidth;
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
