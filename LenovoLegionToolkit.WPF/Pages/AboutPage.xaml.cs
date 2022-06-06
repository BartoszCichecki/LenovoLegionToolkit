using System.Diagnostics;
using System.Reflection;
using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AboutPage
    {
        private string VersionText => Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(3) ?? "";

        private string CopyrightText
        {
            get
            {
                var location = Assembly.GetEntryAssembly()?.Location;
                if (location is null)
                    return "";
                var versionInfo = FileVersionInfo.GetVersionInfo(location);
                return versionInfo.LegalCopyright ?? "";
            }
        }

        public AboutPage()
        {
            InitializeComponent();

            _version.Text += VersionText;
            _copyright.Text = CopyrightText;
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Constants.PayPalUri.Open();
            e.Handled = true;
        }
    }
}
