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

        private readonly StackPanel _stackPanel = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
        };

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

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _subtitleTextBlock.SetResourceReference(ForegroundProperty, "TextFillColorTertiaryBrush");

            _stackPanel.Children.Add(_titleTextBlock);
            _stackPanel.Children.Add(_subtitleTextBlock);

            Content = _stackPanel;
        }
    }
}
