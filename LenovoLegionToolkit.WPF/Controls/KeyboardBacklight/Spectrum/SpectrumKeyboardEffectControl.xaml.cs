using System;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum;

public partial class SpectrumKeyboardEffectControl
{
    public new SpectrumKeyboardBacklightEffect Effect { get; }

    public event EventHandler? Click;
    public event EventHandler? Edit;
    public event EventHandler? Delete;

    public SpectrumKeyboardEffectControl(SpectrumKeyboardBacklightEffect effect)
    {
        Effect = effect;

        InitializeComponent();

        _cardHeaderControl.Title = effect.Type.GetDisplayName();

        var subtitle = string.Empty;
        if (effect.Keys.All)
            subtitle += Resource.SpectrumKeyboardEffectControl_Description_AllZones;
        else
            subtitle += string.Format(Resource.SpectrumKeyboardEffectControl_Description_Zones, effect.Keys.KeyCodes.Length);
        _cardHeaderControl.Subtitle = subtitle;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, EventArgs.Empty);
        e.Handled = true;
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        Edit?.Invoke(this, EventArgs.Empty);
        e.Handled = true;
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        Delete?.Invoke(this, EventArgs.Empty);
        e.Handled = true;
    }
}