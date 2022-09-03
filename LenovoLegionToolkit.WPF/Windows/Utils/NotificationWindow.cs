using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Utils
{
    public class NotificationWindow : UiWindow
    {
        public readonly Grid _mainGrid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = GridLength.Auto, },
                new() { Width = new(1, GridUnitType.Star) },
            },
            Margin = new(16, 16, 16, 16),
        };

        public readonly SymbolIcon _symbolIcon = new()
        {
            FontSize = 32,
            Margin = new(0, 0, 16, 0),
        };

        public readonly SymbolIcon _overlaySymbolIcon = new()
        {
            FontSize = 32,
            Margin = new(0, 0, 16, 0),
        };

        public readonly Label _textBlock = new()
        {
            FontSize = 16,
            FontWeight = FontWeights.Medium,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        public NotificationWindow(SymbolRegular symbol, SymbolRegular? overlaySymbol, string text)
        {
            InitializeStyle();
            InitializePosition();
            InitializeContent(symbol, overlaySymbol, text);

            MouseDown += (s, e) => Close();
        }

        public void Show(int closeAfter)
        {
            Show();
            Task.Delay(closeAfter).ContinueWith(_ =>
            {
                Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void InitializeStyle()
        {
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.Manual;
            ResizeMode = ResizeMode.NoResize;

            Topmost = true;
            ExtendsContentIntoTitleBar = true;
            ShowInTaskbar = false;
            ShowActivated = false;

            _textBlock.Foreground = (SolidColorBrush)FindResource("TextFillColorPrimaryBrush");
        }

        private void InitializePosition()
        {
            Width = MinWidth = 300;
            Height = MinHeight = 80;

            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 16;
            Top = desktopWorkingArea.Bottom - Height - 16;
        }

        private void InitializeContent(SymbolRegular symbol, SymbolRegular? overlaySymbol, string text)
        {
            _symbolIcon.Symbol = symbol;
            _textBlock.Content = text;

            Grid.SetColumn(_symbolIcon, 0);
            Grid.SetColumn(_textBlock, 1);

            _mainGrid.Children.Add(_symbolIcon);
            _mainGrid.Children.Add(_textBlock);

            if (overlaySymbol.HasValue)
            {
                _overlaySymbolIcon.Symbol = overlaySymbol.Value;
                Grid.SetColumn(_overlaySymbolIcon, 0);
                _mainGrid.Children.Add(_overlaySymbolIcon);
            }

            Content = _mainGrid;
        }
    }
}
