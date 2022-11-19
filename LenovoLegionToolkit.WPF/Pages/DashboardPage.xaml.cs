using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DashboardPage
    {
        private StackPanel[] CollapsedPanels => new[]
        {
            _powerStackPanel,
            _graphicsStackPanel,
            _displayStackPanel,
            _otherStackPanel
        };

        private StackPanel[][] ExpandedPanels => new[]
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

        public DashboardPage() => InitializeComponent();

        private void DashboardPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (e.NewSize.Width > 1000)
                Expand();
            else
                Collapse();
        }

        private void Expand()
        {
            for (var row = 0; row < ExpandedPanels.Length; row++)
                for (var column = 0; column < ExpandedPanels[row].Length; column++)
                {
                    var panel = ExpandedPanels[row][column];
                    Grid.SetRow(panel, row);
                    Grid.SetColumn(panel, column);
                }

            _column1.Width = new(1, GridUnitType.Star);
        }

        private void Collapse()
        {
            for (var row = 0; row < CollapsedPanels.Length; row++)
            {
                var panel = CollapsedPanels[row];
                Grid.SetRow(panel, row);
                Grid.SetColumn(panel, 0);
            }

            _column1.Width = new(0, GridUnitType.Pixel);
        }
    }
}
