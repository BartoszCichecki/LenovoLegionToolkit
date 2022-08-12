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
            VerticalAlignment = VerticalAlignment.Center,
            TextTrimming = TextTrimming.CharacterEllipsis,
        };

        private readonly TextBlock _subtitleTextBlock = new()
        {
            FontSize = 12,
            Margin = new(0, 4, 0, 0),
            Visibility = Visibility.Collapsed,
            TextWrapping = TextWrapping.Wrap,
            TextTrimming = TextTrimming.CharacterEllipsis,
        };

        private readonly Grid _grid = new()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new(1, GridUnitType.Star) },
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
            set
            {
                _titleTextBlock.Text = value;
                RefreshLayout();
            }
        }

        public string Subtitle
        {
            get => _subtitleTextBlock.Text;
            set
            {
                _subtitleTextBlock.Text = value;
                RefreshLayout();
            }
        }

        public string? SubtitleToolTip
        {
            get => _subtitleTextBlock.ToolTip as string;
            set
            {
                _subtitleTextBlock.ToolTip = value;
                ToolTipService.SetIsEnabled(_subtitleTextBlock, value != null);
                RefreshLayout();
            }
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

            Grid.SetColumn(_titleTextBlock, 0);
            Grid.SetColumn(_subtitleTextBlock, 0);

            Grid.SetRow(_titleTextBlock, 0);
            Grid.SetRow(_subtitleTextBlock, 1);

            _grid.Children.Add(_titleTextBlock);
            _grid.Children.Add(_subtitleTextBlock);

            Content = _grid;

            UpdateTextStyle();
            IsEnabledChanged += (s, e) => UpdateTextStyle();
        }

        private void RefreshLayout()
        {
            if (string.IsNullOrWhiteSpace(Subtitle))
            {
                Grid.SetRowSpan(_titleTextBlock, 2);
                _subtitleTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                Grid.SetRowSpan(_titleTextBlock, 1);
                _subtitleTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void UpdateTextStyle()
        {
            if (IsEnabled)
            {
                _titleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorPrimaryBrush");
                _subtitleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorTertiaryBrush");
            }
            else
            {
                _titleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorDisabledBrush");
                _subtitleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorDisabledBrush");
            }
        }
    }
}
