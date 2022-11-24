using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MenuItem = System.Windows.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.RGB
{
    public partial class RGBKeyboardBacklightControl
    {
        private Button[] PresetButtons => new[] { _offPresetButton, _preset1Button, _preset2Button, _preset3Button };

        private ColorPickerControl[] Zones => new[] { _zone1ColorPicker, _zone2ColorPicker, _zone3ColorPicker, _zone4ColorPicker };

        private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();
        private readonly RGBKeyboardBacklightListener _listener = IoCContainer.Resolve<RGBKeyboardBacklightListener>();
        private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();

        protected override bool DisablesWhileRefreshing => false;

        public RGBKeyboardBacklightControl()
        {
            InitializeComponent();

            _listener.Changed += Listener_Changed;

            SizeChanged += RGBKeyboardBacklightControl_SizeChanged;
        }

        private void Listener_Changed(object? sender, RGBKeyboardBacklightChanged e) => Dispatcher.Invoke(async () =>
        {
            if (!IsLoaded || !IsVisible)
                return;

            await RefreshAsync();
        });

        private void RGBKeyboardBacklightControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (e.NewSize.Width > 950)
                Expand();
            else
                Collapse();
        }

        private async void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button presetButton || presetButton.Appearance == ControlAppearance.Primary)
                return;

            var selectedPreset = (RGBKeyboardBacklightPreset)presetButton.Tag;
            var state = await _controller.GetStateAsync();
            await _controller.SetStateAsync(new(selectedPreset, state.Presets));

            await RefreshAsync();
        }

        private async void SynchroniseZonesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { Parent: ContextMenu { PlacementTarget: CardControl { Content: ColorPickerControl pickerControl } } })
                return;

            foreach (var zone in Zones)
                zone.SelectedColor = pickerControl.SelectedColor;

            await SaveState();
            await RefreshAsync();
        }

        private async void CardControl_Changed(object? sender, EventArgs e)
        {
            await SaveState();
            await RefreshAsync();
        }

        protected override async Task OnRefreshAsync()
        {
            if (!await _controller.IsSupportedAsync())
                throw new InvalidOperationException("RGB Keyboard does not seem to be supported");

            var vantageStatus = await _vantage.GetStatusAsync();
            if (vantageStatus == SoftwareStatus.Enabled)
            {
                _vantageWarningCard.Visibility = Visibility.Visible;

                _offPresetButton.IsEnabled = false;
                _preset1Button.IsEnabled = false;
                _preset2Button.IsEnabled = false;
                _preset3Button.IsEnabled = false;

                _brightnessControl.IsEnabled = false;
                _effectControl.IsEnabled = false;

                _zone1ColorPicker.Visibility = Visibility.Hidden;
                _zone2ColorPicker.Visibility = Visibility.Hidden;
                _zone3ColorPicker.Visibility = Visibility.Hidden;
                _zone4ColorPicker.Visibility = Visibility.Hidden;

                _speedControl.IsEnabled = false;
                _zone1Control.IsEnabled = false;
                _zone2Control.IsEnabled = false;
                _zone3Control.IsEnabled = false;
                _zone4Control.IsEnabled = false;

                Visibility = Visibility.Visible;

                return;
            }

            var state = await _controller.GetStateAsync();

            foreach (var presetButton in PresetButtons)
            {
                var buttonPreset = (RGBKeyboardBacklightPreset)presetButton.Tag;
                var selected = state.SelectedPreset == buttonPreset;
                presetButton.Appearance = selected ? ControlAppearance.Primary : ControlAppearance.Secondary;
            }

            _vantageWarningCard.Visibility = Visibility.Collapsed;

            _offPresetButton.IsEnabled = true;
            _preset1Button.IsEnabled = true;
            _preset2Button.IsEnabled = true;
            _preset3Button.IsEnabled = true;

            if (state.SelectedPreset == RGBKeyboardBacklightPreset.Off)
            {
                _effectControl.IsEnabled = false;
                _speedControl.IsEnabled = false;
                _brightnessControl.IsEnabled = false;

                _zone1ColorPicker.Visibility = Visibility.Hidden;
                _zone2ColorPicker.Visibility = Visibility.Hidden;
                _zone3ColorPicker.Visibility = Visibility.Hidden;
                _zone4ColorPicker.Visibility = Visibility.Hidden;

                _zone1Control.IsEnabled = false;
                _zone2Control.IsEnabled = false;
                _zone3Control.IsEnabled = false;
                _zone4Control.IsEnabled = false;

                return;
            }

            var preset = state.Presets[state.SelectedPreset];

            var speedEnabled = preset.Effect != RGBKeyboardBacklightEffect.Static;
            var zonesEnabled = preset.Effect == RGBKeyboardBacklightEffect.Static || preset.Effect == RGBKeyboardBacklightEffect.Breath;

            _brightnessControl.SetItems(Enum.GetValues<RGBKeyboardBacklightBrightness>(), preset.Brightness, v => v.GetDisplayName());
            _effectControl.SetItems(Enum.GetValues<RGBKeyboardBacklightEffect>(), preset.Effect, v => v.GetDisplayName());
            if (speedEnabled)
                _speedControl.SetItems(Enum.GetValues<RBGKeyboardBacklightSpeed>(), preset.Speed, v => v.GetDisplayName());

            if (zonesEnabled)
            {
                _zone1ColorPicker.SelectedColor = preset.Zone1.ToColor();
                _zone2ColorPicker.SelectedColor = preset.Zone2.ToColor();
                _zone3ColorPicker.SelectedColor = preset.Zone3.ToColor();
                _zone4ColorPicker.SelectedColor = preset.Zone4.ToColor();

                _zone1ColorPicker.Visibility = Visibility.Visible;
                _zone2ColorPicker.Visibility = Visibility.Visible;
                _zone3ColorPicker.Visibility = Visibility.Visible;
                _zone4ColorPicker.Visibility = Visibility.Visible;
            }
            else
            {
                _zone1ColorPicker.Visibility = Visibility.Hidden;
                _zone2ColorPicker.Visibility = Visibility.Hidden;
                _zone3ColorPicker.Visibility = Visibility.Hidden;
                _zone4ColorPicker.Visibility = Visibility.Hidden;
            }

            _brightnessControl.IsEnabled = true;
            _effectControl.IsEnabled = true;
            _speedControl.IsEnabled = speedEnabled;

            _zone1Control.IsEnabled = zonesEnabled;
            _zone2Control.IsEnabled = zonesEnabled;
            _zone3Control.IsEnabled = zonesEnabled;
            _zone4Control.IsEnabled = zonesEnabled;
        }

        protected override void OnFinishedLoading() { }

        private async Task SaveState()
        {
            var state = await _controller.GetStateAsync();

            var selectedPreset = state.SelectedPreset;
            var presets = state.Presets;

            if (selectedPreset == RGBKeyboardBacklightPreset.Off)
                return;

            presets[selectedPreset] = new(_effectControl.SelectedItem,
                                          _speedControl.SelectedItem,
                                          _brightnessControl.SelectedItem,
                                          _zone1ColorPicker.SelectedColor.ToRGBColor(),
                                          _zone2ColorPicker.SelectedColor.ToRGBColor(),
                                          _zone3ColorPicker.SelectedColor.ToRGBColor(),
                                          _zone4ColorPicker.SelectedColor.ToRGBColor());

            await _controller.SetStateAsync(new(selectedPreset, presets));
        }

        private void Expand()
        {
            Grid.SetColumn(_zone1Control, 0);
            Grid.SetColumn(_zone2Control, 1);
            Grid.SetColumn(_zone3Control, 2);
            Grid.SetColumn(_zone4Control, 3);

            Grid.SetRow(_zone1Control, 5);
            Grid.SetRow(_zone2Control, 5);
            Grid.SetRow(_zone3Control, 5);
            Grid.SetRow(_zone4Control, 5);

            Grid.SetColumnSpan(_zone1Control, 1);
            Grid.SetColumnSpan(_zone2Control, 1);
            Grid.SetColumnSpan(_zone3Control, 1);
            Grid.SetColumnSpan(_zone4Control, 1);
        }

        private void Collapse()
        {
            Grid.SetColumn(_zone1Control, 0);
            Grid.SetColumn(_zone2Control, 0);
            Grid.SetColumn(_zone3Control, 0);
            Grid.SetColumn(_zone4Control, 0);

            Grid.SetRow(_zone1Control, 5);
            Grid.SetRow(_zone2Control, 6);
            Grid.SetRow(_zone3Control, 7);
            Grid.SetRow(_zone4Control, 8);

            Grid.SetColumnSpan(_zone1Control, 4);
            Grid.SetColumnSpan(_zone2Control, 4);
            Grid.SetColumnSpan(_zone3Control, 4);
            Grid.SetColumnSpan(_zone4Control, 4);
        }
    }
}
