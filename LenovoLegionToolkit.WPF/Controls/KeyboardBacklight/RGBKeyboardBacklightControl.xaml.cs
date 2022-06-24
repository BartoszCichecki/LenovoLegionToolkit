using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public partial class RGBKeyboardBacklightControl
    {
        private Button[] PresetButtons => new[] { _offPresetButton, _preset1Button, _preset2Button, _preset3Button };

        private RGBColorKeyboardBacklightCardControl[] Zones => new[] { _zone1Control, _zone2Control, _zone3Control, _zone4Control };

        private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();
        private readonly RGBKeyboardBacklightListener _listener = IoCContainer.Resolve<RGBKeyboardBacklightListener>();
        private readonly Vantage _vantage = IoCContainer.Resolve<Vantage>();

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
            if (sender is not MenuItem menuItem || menuItem.Parent is not ContextMenu menu || menu.PlacementTarget is not RGBColorKeyboardBacklightCardControl control)
                return;

            foreach (var zone in Zones)
                zone.Set(control.SelectedColor);

            await SaveState();
            await RefreshAsync();
        }

        private async void CardControl_OnChanged(object? sender, EventArgs e)
        {
            await SaveState();
            await RefreshAsync();
        }

        protected override async Task OnRefreshAsync()
        {
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

                _zone1Control.IsEnabled = false;
                _zone2Control.IsEnabled = false;
                _zone3Control.IsEnabled = false;
                _zone4Control.IsEnabled = false;

                return;
            }

            var preset = state.Presets[state.SelectedPreset];

            var speedEnabled = preset.Effect != RGBKeyboardEffect.Static;
            var zonesEnabled = preset.Effect == RGBKeyboardEffect.Static || preset.Effect == RGBKeyboardEffect.Breath;

            _brightnessControl.SetItems(Enum.GetValues<RGBKeyboardBrightness>(), preset.Brightness, v => v.GetDisplayName());
            _effectControl.SetItems(Enum.GetValues<RGBKeyboardEffect>(), preset.Effect, v => v.GetDisplayName());
            if (speedEnabled)
                _speedControl.SetItems(Enum.GetValues<RBGKeyboardSpeed>(), preset.Speed, v => v.GetDisplayName());

            if (zonesEnabled)
            {
                _zone1Control.Set(preset.Zone1);
                _zone2Control.Set(preset.Zone2);
                _zone3Control.Set(preset.Zone3);
                _zone4Control.Set(preset.Zone4);
            }
            else
            {
                _zone1Control.Clear();
                _zone2Control.Clear();
                _zone3Control.Clear();
                _zone4Control.Clear();
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
                                          _zone1Control.SelectedColor,
                                          _zone2Control.SelectedColor,
                                          _zone3Control.SelectedColor,
                                          _zone4Control.SelectedColor);

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
