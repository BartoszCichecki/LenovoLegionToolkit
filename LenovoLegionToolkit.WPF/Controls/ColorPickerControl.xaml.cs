using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace LenovoLegionToolkit.WPF.Controls;

public partial class ColorPickerControl
{
    private bool CanHandleEvent => !_isEditing && _colorPicker is not null && _redNumberBox is not null && _greenNumberBox is not null && _blueNumberBox is not null && _hexTextBox is not null;

    private bool _isEditing;

    public Color SelectedColor
    {
        get => _colorPicker.SelectedColor;
        set => _colorPicker.SelectedColor = value;
    }

    public event EventHandler? ColorChangedContinuous;
    public event EventHandler? ColorChangedDelayed;

    public ColorPickerControl()
    {
        InitializeComponent();

        SelectedColor = Colors.Aqua;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _popup.IsOpen = true;
        e.Handled = true;
    }

    private void ColorPicker_MouseUp(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        ColorChangedDelayed?.Invoke(this, EventArgs.Empty);
    }

    private void ColorPicker_MouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void ColorPicker_ColorChanged(object sender, RoutedEventArgs e)
    {
        if (!CanHandleEvent)
            return;

        _isEditing = true;

        var color = _colorPicker.SelectedColor;

        _button.Background = new SolidColorBrush(color);

        _redNumberBox.Text = color.R.ToString();
        _greenNumberBox.Text = color.G.ToString();
        _blueNumberBox.Text = color.B.ToString();

        _hexTextBox.Text = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        ColorChangedContinuous?.Invoke(this, EventArgs.Empty);

        _isEditing = false;
    }


    private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!CanHandleEvent)
            return;

        _isEditing = true;

        var r = ToByte(_redNumberBox.Text);
        var g = ToByte(_greenNumberBox.Text);
        var b = ToByte(_blueNumberBox.Text);
        var color = Color.FromRgb(r, g, b);

        _button.Background = new SolidColorBrush(color);

        _hexTextBox.Text = $"#{r:X2}{g:X2}{b:X2}";

        if (Mouse.LeftButton != MouseButtonState.Pressed && Mouse.RightButton != MouseButtonState.Pressed)
        {
            _colorPicker.SelectedColor = color;

            ColorChangedDelayed?.Invoke(this, EventArgs.Empty);
        }

        _isEditing = false;
    }

    private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!CanHandleEvent)
            return;

        if (!Regex.Match(_hexTextBox.Text, "^#(?:[0-9A-F]{3}){2}$", RegexOptions.IgnoreCase).Success)
            return;

        _isEditing = true;

        try
        {
            var c = ColorTranslator.FromHtml(_hexTextBox.Text);
            var color = Color.FromRgb(c.R, c.G, c.B);

            _button.Background = new SolidColorBrush(color);

            _redNumberBox.Text = color.R.ToString();
            _greenNumberBox.Text = color.G.ToString();
            _blueNumberBox.Text = color.B.ToString();

            if (Mouse.LeftButton != MouseButtonState.Pressed && Mouse.RightButton != MouseButtonState.Pressed)
            {
                _colorPicker.SelectedColor = color;

                ColorChangedDelayed?.Invoke(this, EventArgs.Empty);
            }
        }
        catch { }

        _isEditing = false;
    }

    private void OK_Click(object sender, RoutedEventArgs e) => _popup.IsOpen = false;

    private static byte ToByte(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return 0;

        return (byte)Math.Clamp(Convert.ToInt32(s), 0, 255);
    }
}