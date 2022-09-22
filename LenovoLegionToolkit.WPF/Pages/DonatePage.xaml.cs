using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DonatePage
    {
        public DonatePage()
        {
            InitializeComponent();
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Constants.PayPalUri.Open();
            e.Handled = true;
        }
    }
}
