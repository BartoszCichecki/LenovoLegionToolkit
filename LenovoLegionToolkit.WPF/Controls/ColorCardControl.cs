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

namespace LenovoLegionToolkit.WPF.Controls
{
    public class ColorCardControl : UserControl
    {
        private readonly CardExpander _cardExpander = new();

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly Button _colorButton = new()
        {
            Width = 48,
            Height = 24,
            Margin = new(0, 0, 8, 0),
        };

        private readonly StackPanel _colorsPanel = new()
        {
            Width = 196,
        };

        private readonly SquarePicker _colorPicker = new()
        {
            Height = 196,
        };

        private readonly Grid _rgbGrid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = GridLength.Auto },
                new() { Width = new(16, GridUnitType.Pixel) },
                new() { Width = new(1, GridUnitType.Star) },
            },
            RowDefinitions =
            {
                new() { Height = GridLength.Auto },
                new() { Height = GridLength.Auto },
                new() { Height = GridLength.Auto },
            },
        };

        private readonly Label _redLabel = new()
        {
            Content = "Red:",
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new(0, 8, 0, 0),
        };

        private readonly Label _greenLabel = new()
        {
            Content = "Green:",
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new(0, 8, 0, 0),
        };

        private readonly Label _blueLabel = new()
        {
            Content = "Blue:",
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new(0, 8, 0, 0),
        };

        private readonly NumberBox _redTextBox = new()
        {
            Min = 0,
            Max = 255,
            IntegersOnly = true,
            ClearButtonEnabled = false,
            Margin = new(0, 8, 0, 0),
        };

        private readonly NumberBox _greenTextBox = new()
        {
            Min = 0,
            Max = 255,
            IntegersOnly = true,
            ClearButtonEnabled = false,
            Margin = new(0, 8, 0, 0),
        };

        private readonly NumberBox _blueTextBox = new()
        {
            Min = 0,
            Max = 255,
            IntegersOnly = true,
            ClearButtonEnabled = false,
            Margin = new(0, 8, 0, 0),
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

        public event EventHandler? OnChanged, OnChangedByUser;

        public ColorCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            _colorPicker.ColorChanged += ColorPicker_ColorChanged;
            _redTextBox.TextChanged += ColorTextBox_TextChanged;
            _greenTextBox.TextChanged += ColorTextBox_TextChanged;
            _blueTextBox.TextChanged += ColorTextBox_TextChanged;
            _colorPicker.MouseUp += ColorsPanel_MouseUp;

            _colorButton.Click += (s, e) => _cardExpander.IsExpanded = !_cardExpander.IsExpanded;
            _cardHeaderControl.Accessory = _colorButton;

            Grid.SetColumn(_redLabel, 0);
            Grid.SetColumn(_greenLabel, 0);
            Grid.SetColumn(_blueLabel, 0);
            Grid.SetColumn(_redTextBox, 2);
            Grid.SetColumn(_greenTextBox, 2);
            Grid.SetColumn(_blueTextBox, 2);

            Grid.SetRow(_redLabel, 0);
            Grid.SetRow(_greenLabel, 1);
            Grid.SetRow(_blueLabel, 2);
            Grid.SetRow(_redTextBox, 0);
            Grid.SetRow(_greenTextBox, 1);
            Grid.SetRow(_blueTextBox, 2);

            _rgbGrid.Children.Add(_redLabel);
            _rgbGrid.Children.Add(_greenLabel);
            _rgbGrid.Children.Add(_blueLabel);
            _rgbGrid.Children.Add(_redTextBox);
            _rgbGrid.Children.Add(_greenTextBox);
            _rgbGrid.Children.Add(_blueTextBox);

            _colorsPanel.Children.Add(_colorPicker);
            _colorsPanel.Children.Add(_rgbGrid);

            _cardExpander.Header = _cardHeaderControl;
            _cardExpander.Content = GetCardExpanderPanel();

            Content = _cardExpander;
        }

        protected virtual StackPanel GetCardExpanderPanel() => _colorsPanel;

        //
        // These flags are used to prevent ColorTextBox_TextChanged from raising OnChanged
        // repeatedly when SetColor is called.
        //
        private bool _wasSetColorCalled = false, _raisedOnChanged = false;

        private void ColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var result = byte.TryParse(_redTextBox.Text, out var r);
            result &= byte.TryParse(_greenTextBox.Text, out var g);
            result &= byte.TryParse(_blueTextBox.Text, out var b);

            if (!result)
                return;

            var color = Color.FromRgb(r, g, b);

            if (Color.Equals(color, _colorPicker.SelectedColor))
                return;

            _colorPicker.SelectedColor = Color.FromRgb(r, g, b);

            if (_wasSetColorCalled && _raisedOnChanged)
                return;

            if (Mouse.LeftButton == MouseButtonState.Released && Mouse.RightButton == MouseButtonState.Released)
            {
                OnChanged?.Invoke(this, EventArgs.Empty);

                if (!_wasSetColorCalled)
                    OnChangedByUser?.Invoke(this, EventArgs.Empty);

                _raisedOnChanged = true;
            }
        }

        private void ColorPicker_ColorChanged(object sender, RoutedEventArgs e)
        {
            var color = _colorPicker.SelectedColor;
            _redTextBox.Text = color.R.ToString();
            _greenTextBox.Text = color.G.ToString();
            _blueTextBox.Text = color.B.ToString();
            _colorButton.Background = new SolidColorBrush(color);
        }

        private void ColorsPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
            OnChangedByUser?.Invoke(this, EventArgs.Empty);
        }

        public void SetColor(RGBColor color)
        {
            _wasSetColorCalled = true;
            _raisedOnChanged = false;

            var c = Color.FromRgb(color.R, color.G, color.B);
            _redTextBox.Text = color.R.ToString();
            _greenTextBox.Text = color.G.ToString();
            _blueTextBox.Text = color.B.ToString();
            _colorPicker.SelectedColor = c;
            _colorButton.Background = new SolidColorBrush(c);
            _colorButton.Visibility = Visibility.Visible;

            _wasSetColorCalled = _raisedOnChanged = false;
        }

        public RGBColor GetColor()
        {
            var selectedColor = _colorPicker.SelectedColor;
            return new(selectedColor.R, selectedColor.G, selectedColor.B);
        }

        public void Clear()
        {
            _colorButton.Visibility = Visibility.Hidden;
            _cardExpander.IsExpanded = false;
        }
    }
}
