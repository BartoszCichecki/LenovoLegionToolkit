using System.Diagnostics;
using System.Reflection;

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
    }
}
