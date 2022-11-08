using System.Globalization;
using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class DonatePage
    {
        public DonatePage()
        {
            InitializeComponent();

            if (Resource.Culture.Equals(new CultureInfo("zh-hans")))
            {
                _paypal.Visibility = Visibility.Collapsed;
                _stripeChina.Visibility = Visibility.Visible;
            }
            else
            {
                _paypal.Visibility = Visibility.Visible;
                _stripeChina.Visibility = Visibility.Collapsed;
            }
        }

        private void PayPalDonateButton_Click(object sender, RoutedEventArgs e)
        {
            Constants.PayPalUri.Open();
            e.Handled = true;
        }

        private void StripeCNYDonateButton_Click(object sender, RoutedEventArgs e)
        {
            Constants.StripeCNYUri.Open();
            e.Handled = true;
        }
    }
}
