﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Windows.KeyboardBacklight.Spectrum;

public partial class SpectrumKeyboardBacklightEditEffectWindow
{
    private readonly ushort[] _keyCodes;
    private readonly ushort[] _allKeyboardKeyCodes;

    public event EventHandler<SpectrumKeyboardBacklightEffect>? Apply;

    public SpectrumKeyboardBacklightEditEffectWindow(ushort[] keyCodes, ushort[] allKeyboardKeyCodes)
    {
        _keyCodes = keyCodes;
        _allKeyboardKeyCodes = allKeyboardKeyCodes;

        InitializeComponent();

        _title.Text = Resource.SpectrumKeyboardBacklightEditEffectWindow_Title_Add;

        SetInitialValues();
        RefreshVisibility();
    }

    public SpectrumKeyboardBacklightEditEffectWindow(SpectrumKeyboardBacklightEffect effect, ushort[] keyCodes, ushort[] allKeyboardKeyCodes)
    {
        _keyCodes = effect.Type.IsAllLightsEffect() ? keyCodes : effect.Keys;
        _allKeyboardKeyCodes = allKeyboardKeyCodes;

        InitializeComponent();

        ResizeMode = ResizeMode.CanMinimize;

        _title.Text = Resource.SpectrumKeyboardBacklightEditEffectWindow_Title_Add;

        _titleBar.UseSnapLayout = false;
        _titleBar.CanMaximize = false;

        SetInitialValues();
        Update(effect);
        RefreshVisibility();
    }

    private void EffectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => RefreshVisibility();

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        var effectType = SpectrumKeyboardBacklightEffectType.Always;
        var direction = SpectrumKeyboardBacklightDirection.None;
        var clockwiseDirection = SpectrumKeyboardBacklightClockwiseDirection.None;
        var speed = SpectrumKeyboardBacklightSpeed.None;
        var colors = Array.Empty<RGBColor>();

        if (_effectTypeCard.Visibility == Visibility.Visible &&
            _effectTypeComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightEffectType effectTypeTemp))
            effectType = effectTypeTemp;

        if (_directionCard.Visibility == Visibility.Visible &&
            _directionComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightDirection directionTemp))
            direction = directionTemp;

        if (_clockwiseDirectionCard.Visibility == Visibility.Visible &&
            _clockwiseDirectionComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightClockwiseDirection clockwiseDirectionTemp))
            clockwiseDirection = clockwiseDirectionTemp;

        if (_speedCard.Visibility == Visibility.Visible &&
            _speedComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightSpeed speedTemp))
            speed = speedTemp;

        if (_singleColor.Visibility == Visibility.Visible)
            colors = [_singleColorPicker.SelectedColor.ToRGBColor()];

        if (_multiColors.Visibility == Visibility.Visible)
            colors = _multiColorPicker.SelectedColors.Select(c => c.ToRGBColor()).ToArray();

        var keys = _keyCodes;

        if (effectType.IsAllLightsEffect())
            keys = [];
        if (effectType.IsWholeKeyboardEffect())
            keys = _allKeyboardKeyCodes;

        var effect = new SpectrumKeyboardBacklightEffect(effectType,
            speed,
            direction,
            clockwiseDirection,
            colors,
            keys);

        Apply?.Invoke(this, effect);
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

    private void SetInitialValues()
    {
        _effectTypeComboBox.SetItems(
            [
                SpectrumKeyboardBacklightEffectType.Always,
                SpectrumKeyboardBacklightEffectType.RainbowScrew,
                SpectrumKeyboardBacklightEffectType.RainbowWave,
                SpectrumKeyboardBacklightEffectType.ColorChange,
                SpectrumKeyboardBacklightEffectType.ColorWave,
                SpectrumKeyboardBacklightEffectType.ColorPulse,
                SpectrumKeyboardBacklightEffectType.Smooth,
                SpectrumKeyboardBacklightEffectType.Rain,
                SpectrumKeyboardBacklightEffectType.Ripple,
                SpectrumKeyboardBacklightEffectType.Type,
                SpectrumKeyboardBacklightEffectType.AudioBounce,
                SpectrumKeyboardBacklightEffectType.AudioRipple,
                SpectrumKeyboardBacklightEffectType.AuroraSync
            ],
            SpectrumKeyboardBacklightEffectType.Always,
            e => e.GetDisplayName());

        _directionComboBox.SetItems(
            [
                SpectrumKeyboardBacklightDirection.BottomToTop,
                SpectrumKeyboardBacklightDirection.TopToBottom,
                SpectrumKeyboardBacklightDirection.LeftToRight,
                SpectrumKeyboardBacklightDirection.RightToLeft
            ],
            SpectrumKeyboardBacklightDirection.BottomToTop,
            e => e.GetDisplayName());

        _clockwiseDirectionComboBox.SetItems(
            [
                SpectrumKeyboardBacklightClockwiseDirection.Clockwise,
                SpectrumKeyboardBacklightClockwiseDirection.CounterClockwise
            ],
            SpectrumKeyboardBacklightClockwiseDirection.Clockwise,
            e => e.GetDisplayName());

        _speedComboBox.SetItems(
            [
                SpectrumKeyboardBacklightSpeed.Speed1,
                SpectrumKeyboardBacklightSpeed.Speed2,
                SpectrumKeyboardBacklightSpeed.Speed3
            ],
            SpectrumKeyboardBacklightSpeed.Speed2,
            e => e.GetDisplayName());
    }

    private void Update(SpectrumKeyboardBacklightEffect effect)
    {
        if (_effectTypeComboBox.GetItems<SpectrumKeyboardBacklightEffectType>().Contains(effect.Type))
            _effectTypeComboBox.SelectItem(effect.Type);

        if (_directionComboBox.GetItems<SpectrumKeyboardBacklightDirection>().Contains(effect.Direction))
            _directionComboBox.SelectItem(effect.Direction);

        if (_clockwiseDirectionComboBox.GetItems<SpectrumKeyboardBacklightClockwiseDirection>()
            .Contains(effect.ClockwiseDirection))
            _clockwiseDirectionComboBox.SelectItem(effect.ClockwiseDirection);

        if (_speedComboBox.GetItems<SpectrumKeyboardBacklightSpeed>().Contains(effect.Speed))
            _speedComboBox.SelectItem(effect.Speed);

        var colors = effect.Colors.Select(c => Color.FromRgb(c.R, c.G, c.B)).ToArray();
        if (colors.Length != 0)
        {
            _singleColorPicker.SelectedColor = colors.First();
            _multiColorPicker.SelectedColors = colors;
        }
    }

    private void RefreshVisibility()
    {
        if (!_effectTypeComboBox.TryGetSelectedItem(out SpectrumKeyboardBacklightEffectType effect))
            return;

        _effectTypeCardHeader.Warning = effect.IsAllLightsEffect() || effect.IsWholeKeyboardEffect()
            ? Resource.SpectrumKeyboardBacklightEditEffectWindow_Effect_Warning
            : string.Empty;

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

        _multiColors.Visibility = effect switch
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
