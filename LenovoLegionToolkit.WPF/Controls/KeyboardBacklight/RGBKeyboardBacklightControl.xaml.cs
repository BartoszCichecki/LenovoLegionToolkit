using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.Lib.Extensions;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public partial class RGBKeyboardBacklightControl
    {
        private Button[] PresetButtons => new[] { _offPresetButton, _preset1Button, _preset2Button, _preset3Button };

        private RGBColorKeyboardBacklightCardControl[] Zones => new[] { _zone1Control, _zone2Control, _zone3Control, _zone4Control };

        private readonly RGBKeyboardBacklightController _controller = IoCContainer.Resolve<RGBKeyboardBacklightController>();

        public RGBKeyboardBacklightControl()
        {
            InitializeComponent();

            Loaded += RGBKeyboardBacklightControl_Loaded;
            IsVisibleChanged += RGBKeyboardBacklightControl_IsVisibleChanged;
            SizeChanged += RGBKeyboardBacklightControl_SizeChanged;
        }

        private async void RGBKeyboardBacklightControl_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(250);
            await RefreshAsync();
            await loadingTask;
        }

        private async void RGBKeyboardBacklightControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private void RGBKeyboardBacklightControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (e.NewSize.Width > 1000)
                Expand();
            else
                Collapse();
        }

        private async void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button presetButton || presetButton.Appearance == ControlAppearance.Primary)
                return;

            var state = await _controller.GetStateAsync();
            var index = int.Parse((string)presetButton.Tag);
            await _controller.SetStateAsync(new(index, state.Presets));

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

        private async Task RefreshAsync()
        {
            try
            {
                var state = await _controller.GetStateAsync();

                foreach (var presetButton in PresetButtons)
                {
                    var index = int.Parse((string)presetButton.Tag);
                    var selected = state.ActivePresetIndex == index;
                    presetButton.Appearance = selected ? ControlAppearance.Primary : ControlAppearance.Secondary;
                }

                if (state.ActivePresetIndex < 0)
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

                var preset = state.Presets[state.ActivePresetIndex];

                var speedEnabled = preset.Effect != RGBKeyboardEffect.Static;
                var zonesEnabled = preset.Effect == RGBKeyboardEffect.Static || preset.Effect == RGBKeyboardEffect.Breath;

                _brightnessControl.IsEnabled = true;
                _effectControl.IsEnabled = true;
                _speedControl.IsEnabled = speedEnabled;

                _zone1Control.IsEnabled = zonesEnabled;
                _zone2Control.IsEnabled = zonesEnabled;
                _zone3Control.IsEnabled = zonesEnabled;
                _zone4Control.IsEnabled = zonesEnabled;

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
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }

        private async Task SaveState()
        {
            var state = await _controller.GetStateAsync();

            var index = state.ActivePresetIndex;
            var presets = state.Presets;

            if (index < 0)
                return;

            presets[index] = new(_effectControl.SelectedItem,
                                 _speedControl.SelectedItem,
                                 _brightnessControl.SelectedItem,
                                 _zone1Control.SelectedColor,
                                 _zone2Control.SelectedColor,
                                 _zone3Control.SelectedColor,
                                 _zone4Control.SelectedColor);

            await _controller.SetStateAsync(new(index, presets));
        }

        private void Expand()
        {
            Grid.SetColumn(_zone1Control, 0);
            Grid.SetColumn(_zone2Control, 1);
            Grid.SetColumn(_zone3Control, 2);
            Grid.SetColumn(_zone4Control, 3);

            Grid.SetRow(_zone1Control, 4);
            Grid.SetRow(_zone2Control, 4);
            Grid.SetRow(_zone3Control, 4);
            Grid.SetRow(_zone4Control, 4);

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

            Grid.SetRow(_zone1Control, 4);
            Grid.SetRow(_zone2Control, 5);
            Grid.SetRow(_zone3Control, 6);
            Grid.SetRow(_zone4Control, 7);

            Grid.SetColumnSpan(_zone1Control, 4);
            Grid.SetColumnSpan(_zone2Control, 4);
            Grid.SetColumnSpan(_zone3Control, 4);
            Grid.SetColumnSpan(_zone4Control, 4);
        }
    }
}
