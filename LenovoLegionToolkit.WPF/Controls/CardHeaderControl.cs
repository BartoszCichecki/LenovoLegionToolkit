using System;
using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public class CardHeaderControl : UserControl
    {
        private readonly TextBlock _titleTextBlock = new()
        {
            FontSize = 14,
            FontWeight = FontWeight.FromOpenTypeWeight(500), // Medium
        };

        private readonly TextBlock _subtitleTextBlock = new()
        {
            FontSize = 12,
            Margin = new(0, 4, 0, 0),
        };

        private readonly Grid _grid = new()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto },
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
            },
        };

        private UIElement? _accessory;

        public string Title
        {
            get => _titleTextBlock.Text;
            set => _titleTextBlock.Text = value;
        }

        public string Subtitle
        {
            get => _subtitleTextBlock.Text;
            set => _subtitleTextBlock.Text = value;
        }

        public UIElement? Accessory
        {
            get => _accessory;
            set
            {
                if (_accessory is not null)
                    _grid.Children.Remove(_accessory);

                _accessory = value;

                if (_accessory is null)
                    return;

                Grid.SetColumn(_accessory, 1);
                Grid.SetRow(_accessory, 0);
                Grid.SetRowSpan(_accessory, 2);

                _grid.Children.Add(_accessory);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _subtitleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorTertiaryBrush");

            Grid.SetColumn(_titleTextBlock, 0);
            Grid.SetColumn(_subtitleTextBlock, 0);

            Grid.SetRow(_titleTextBlock, 0);
            Grid.SetRow(_subtitleTextBlock, 1);

            _grid.Children.Add(_titleTextBlock);
            _grid.Children.Add(_subtitleTextBlock);

            Content = _grid;
        }
    }
}
