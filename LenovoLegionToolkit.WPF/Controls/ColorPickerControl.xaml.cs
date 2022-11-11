using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColorPicker;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class ColorPickerControl
    {
        public Color SelectedColor
        {
            get => _colorPicker.SelectedColor;
            set => _colorPicker.SelectedColor = value;
        }

        public event EventHandler<EventArgs>? ColorChangedContinuous;
        public event EventHandler<EventArgs>? ColorChangedDelayed;

        public ColorPickerControl()
        {
            InitializeComponent();
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

        private void ColorPicker_ColorChanged(object sender, RoutedEventArgs e) => Update(sender);

        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e) => Update(sender);

        private void Update(object? sender)
        {
            if (_colorPicker is null || _redNumberBox is null || _greenNumberBox is null || _blueNumberBox is null)
                return;

            if (sender is SquarePicker)
            {
                var color = _colorPicker.SelectedColor;
                _redNumberBox.Text = color.R.ToString();
                _greenNumberBox.Text = color.G.ToString();
                _blueNumberBox.Text = color.B.ToString();
                _button.Background = new SolidColorBrush(color);
                ColorChangedContinuous?.Invoke(this, EventArgs.Empty);
            }

            if (sender is NumberBox)
            {
                var r = ToByte(_redNumberBox.Text);
                var g = ToByte(_greenNumberBox.Text);
                var b = ToByte(_blueNumberBox.Text);
                var color = Color.FromRgb(r, g, b);
                _button.Background = new SolidColorBrush(color);

                if (Mouse.LeftButton != MouseButtonState.Pressed && Mouse.RightButton != MouseButtonState.Pressed)
                {
                    _colorPicker.SelectedColor = color;
                    ColorChangedDelayed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private static byte ToByte(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0;

            return (byte)Math.Clamp(Convert.ToInt32(s), 0, 255);
        }
    }
}
