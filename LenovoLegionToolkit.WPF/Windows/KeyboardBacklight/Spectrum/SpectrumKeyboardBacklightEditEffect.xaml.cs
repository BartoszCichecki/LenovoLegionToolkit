using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Windows.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardBacklightEditEffect
    {
        private readonly ushort[] _keys;

        public SpectrumKeyboardBacklightEditEffect(ushort[] keys)
        {
            _keys = keys;

            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            _title.Text = $"Add effect to {keys.Length} keys";

            SetInitialValues();
            RefreshVisibility();
        }

        private void EffectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshVisibility();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var effectType = SpectrumKeyboardBacklightEffectType.Always;
            var direction = SpectrumKeyboardBacklightDirection.None;
            var speed = SpectrumKeyboardBacklightSpeed.None;
            var colors = Array.Empty<RGBColor>();

            if (_effectTypeCard.Visibility == Visibility.Visible &&
                _effectTypeComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightEffectType effectTypeTemp))
                effectType = effectTypeTemp;

            if (_directionCard.Visibility == Visibility.Visible &&
                _directionComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightDirection directionTemp))
                direction = directionTemp;

            if (_clockwiseDirectionCard.Visibility == Visibility.Visible &&
                _clockwiseDirectionComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightDirection clockwiseDirectionTemp))
                direction = clockwiseDirectionTemp;

            if (_speedCard.Visibility == Visibility.Visible &&
                _speedComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightSpeed speedTemp))
                speed = speedTemp;

            if (_singleColor.Visibility == Visibility.Visible)
                colors = new[] { _singleColorPicker.SelectedColor.ToRGBColor() };

            var effect = new SpectrumKeyboardBacklightEffect(effectType,
                speed,
                direction,
                colors,
                _keys);
        }

        private void SetInitialValues()
        {
            _effectTypeComboBox.SetItems(Enum.GetValues<SpectrumKeyboardBacklightEffectType>(),
                SpectrumKeyboardBacklightEffectType.Always,
                e => e.GetDisplayName());

            _directionComboBox.SetItems(new[]
                {
                    SpectrumKeyboardBacklightDirection.BottomToTop,
                    SpectrumKeyboardBacklightDirection.TopToBottom,
                    SpectrumKeyboardBacklightDirection.LeftToRight,
                    SpectrumKeyboardBacklightDirection.RightToLeft
                },
                SpectrumKeyboardBacklightDirection.BottomToTop,
                e => e.GetDisplayName());

            _clockwiseDirectionComboBox.SetItems(new[]
                {
                    SpectrumKeyboardBacklightDirection.Clockwise,
                    SpectrumKeyboardBacklightDirection.CounterClockwise
                },
                SpectrumKeyboardBacklightDirection.Clockwise,
                e => e.GetDisplayName());

            _speedComboBox.SetItems(new[]
                {
                    SpectrumKeyboardBacklightSpeed.Speed1,
                    SpectrumKeyboardBacklightSpeed.Speed2,
                    SpectrumKeyboardBacklightSpeed.Speed3
                },
                SpectrumKeyboardBacklightSpeed.Speed2,
                e => (int)e);

            _singleColorPicker.SelectedColor = Colors.White;
        }

        private void RefreshVisibility()
        {
            if (!_effectTypeComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightEffectType effect))
                return;

            _directionCard.Visibility = effect switch
            {
                SpectrumKeyboardBacklightEffectType.ColorWave => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.RainbowWave => Visibility.Visible,
                _ => Visibility.Collapsed
            };

            _clockwiseDirectionCard.Visibility = effect switch
            {
                SpectrumKeyboardBacklightEffectType.RainbowScrew => Visibility.Visible,
                _ => Visibility.Collapsed
            };

            _speedCard.Visibility = effect switch
            {
                SpectrumKeyboardBacklightEffectType.ColorChange => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.ColorPulse => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.ColorWave => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Rain => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.RainbowScrew => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.RainbowWave => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Ripple => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Smooth => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Type => Visibility.Visible,
                _ => Visibility.Collapsed
            };

            _singleColor.Visibility = effect switch
            {
                SpectrumKeyboardBacklightEffectType.Always => Visibility.Visible,
                _ => Visibility.Collapsed
            };

            _colors.Visibility = effect switch
            {
                SpectrumKeyboardBacklightEffectType.ColorChange => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.ColorPulse => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.ColorWave => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Rain => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Ripple => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Smooth => Visibility.Visible,
                SpectrumKeyboardBacklightEffectType.Type => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }
    }
}
