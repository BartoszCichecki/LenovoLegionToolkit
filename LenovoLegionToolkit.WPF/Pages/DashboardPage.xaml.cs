using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DashboardPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            if (ActualWidth < 1100)
            {
                _column0.Width = new GridLength(1, GridUnitType.Star);
                _column1.Width = new GridLength(0, GridUnitType.Pixel);
                _column2.Width = new GridLength(0, GridUnitType.Pixel);

                Grid.SetColumn(_stackPanelRight, 0);
                Grid.SetRow(_stackPanelRight, 1);
            }
            else
            {
                _column0.Width = new GridLength(1, GridUnitType.Star);
                _column1.Width = new GridLength(16, GridUnitType.Pixel);
                _column2.Width = new GridLength(1, GridUnitType.Star);

                Grid.SetColumn(_stackPanelRight, 2);
                Grid.SetRow(_stackPanelRight, 0);
            }
        }
    }
}
