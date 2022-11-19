using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DashboardPage
    {
        private const int WIDTH_BREAKPOINT = 1000;

        private readonly StackPanel[] _collapsedPanels;

        private readonly StackPanel[][] _expandedPanels;

        public DashboardPage()
        {
            InitializeComponent();

            _collapsedPanels = new[]
            {
                _powerStackPanel,
                _graphicsStackPanel,
                _displayStackPanel,
                _otherStackPanel
            };

            _expandedPanels = new[]
            {
                new[]
                {
                    _powerStackPanel,
                    _graphicsStackPanel
                },
                new []
                {
                    _displayStackPanel,
                    _otherStackPanel
                }
            };
        }

        private void DashboardPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (e.NewSize.Width > WIDTH_BREAKPOINT)
                Expand();
            else
                Collapse();
        }

        private void Expand()
        {
            for (var row = 0; row < _expandedPanels.Length; row++)
                for (var column = 0; column < _expandedPanels[row].Length; column++)
                {
                    var panel = _expandedPanels[row][column];
                    Grid.SetRow(panel, row);
                    Grid.SetColumn(panel, column);
                }

            _column1.Width = new(1, GridUnitType.Star);
        }

        private void Collapse()
        {
            for (var row = 0; row < _collapsedPanels.Length; row++)
            {
                var panel = _collapsedPanels[row];
                Grid.SetRow(panel, row);
                Grid.SetColumn(panel, 0);
            }

            _column1.Width = new(0, GridUnitType.Pixel);
        }
    }
}
