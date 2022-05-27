using System.Windows;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DashboardPage
    {
        public DashboardPage()
        {
            InitializeComponent();

            SizeChanged += DashboardPage_SizeChanged;
        }

        private void DashboardPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;

            if (e.NewSize.Width < 1000)
            {
                _column0.Width = new GridLength(1, GridUnitType.Star);
                _column1.Width = new GridLength(0, GridUnitType.Pixel);
                _column2.Width = new GridLength(0, GridUnitType.Pixel);

                _row1.Height = new GridLength(16, GridUnitType.Pixel);

                Grid.SetColumn(_stackPanelRight, 0);
                Grid.SetRow(_stackPanelRight, 2);
            }
            else
            {
                _column0.Width = new GridLength(1, GridUnitType.Star);
                _column1.Width = new GridLength(16, GridUnitType.Pixel);
                _column2.Width = new GridLength(1, GridUnitType.Star);

                _row1.Height = new GridLength(0, GridUnitType.Pixel);

                Grid.SetColumn(_stackPanelRight, 2);
                Grid.SetRow(_stackPanelRight, 0);
            }
        }
    }
}
