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
    public class ColorKeyboardBacklightCardControl : UserControl
    {
        private readonly CardExpander _cardControl = new();

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
            get => _cardControl.Icon;
            set => _cardControl.Icon = value;
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

        public ColorKeyboardBacklightCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            IsEnabledChanged += CardControl_IsEnabledChanged;

            _colorPicker.ColorChanged += ColorPicker_ColorChanged;
            _colorPicker.MouseUp += ColorPicker_MouseUp;

            _colorButton.Click += (s, e) => RaiseEvent(e);
            _cardHeaderControl.Accessory = _colorButton;

            _cardControl.Header = _cardHeaderControl;
            _cardControl.Content = _colorPicker;
            _cardControl.Margin = new(0, 0, 0, 8);

            Content = _cardControl;
        }

        private void CardControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
                return;

            _cardControl.IsExpanded = false;

            var c = Color.FromRgb(0, 0, 0);
            _colorButton.Visibility = Visibility.Hidden;
            _colorButton.Background = new SolidColorBrush(c);
            _colorPicker.SelectedColor = c;
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
    }
}
