using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AboutPage
    {
        private string VersionText
        {
            get
            {
                if (Configuration.IsBeta)
                    return $"BETA {Configuration.BetaNumber}";

                return Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(3) ?? "";
            }
        }

        private string CopyrightText
        {
            get
            {
                var location = Assembly.GetEntryAssembly()?.Location;
                if (location == null)
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
            Process.Start(new ProcessStartInfo("https://www.paypal.com/donate/?hosted_button_id=22AZE2NBP3HTL") { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
