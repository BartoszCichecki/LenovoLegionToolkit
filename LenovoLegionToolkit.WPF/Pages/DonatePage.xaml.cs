using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class DonatePage
{
    public DonatePage()
    {
        InitializeComponent();
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