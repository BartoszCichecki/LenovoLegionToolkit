using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColorPicker;
using LenovoLegionToolkit.Lib;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public class RGBColorKeyboardBacklightCardControl : UserControl
    {
        private readonly CardExpander _cardExpander = new();

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly Button _colorButton = new()
        {
            Width = 48,
            Height = 24,
            Margin = new(0, 0, 8, 0),
        };

        private readonly SquarePicker _colorPicker = new()
        {
            Width = 196,
            Height = 196,
        };

        public SymbolRegular Icon
        {
            get => _cardExpander.Icon;
            set => _cardExpander.Icon = value;
        }

        public string Title
        {
            get => _cardHeaderControl.Title;
            set => _cardHeaderControl.Title = value;
        }

        public string Subtitle
        {
            get => _cardHeaderControl.Subtitle;
            set => _cardHeaderControl.Subtitle = value;
        }

        public RGBColor SelectedColor
        {
            get
            {
                var selectedColor = _colorPicker.SelectedColor;
                return new(selectedColor.R, selectedColor.G, selectedColor.B);
            }
        }

        public event EventHandler? OnChanged;

        public RGBColorKeyboardBacklightCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            _colorPicker.ColorChanged += ColorPicker_ColorChanged;
            _colorPicker.MouseUp += ColorPicker_MouseUp;

            _colorButton.Click += (s, e) => _cardExpander.IsExpanded = !_cardExpander.IsExpanded;
            _cardHeaderControl.Accessory = _colorButton;

            _cardExpander.Header = _cardHeaderControl;
            _cardExpander.Content = _colorPicker;
            _cardExpander.Margin = new(0, 0, 0, 8);

            Content = _cardExpander;
        }

        private void ColorPicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            _colorButton.Background = new SolidColorBrush(_colorPicker.SelectedColor);
        }

        private void ColorPicker_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnChanged?.Invoke(this, new EventArgs());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => OnChanged?.Invoke(this, EventArgs.Empty);

        public void Set(RGBColor color)
        {
            var c = Color.FromRgb(color.R, color.G, color.B);
            _colorPicker.SelectedColor = c;
            _colorButton.Background = new SolidColorBrush(c);
            _colorButton.Visibility = Visibility.Visible;
        }

        public void Clear()
        {
            _colorButton.Visibility = Visibility.Hidden;
            _cardExpander.IsExpanded = false;
        }
    }
}
