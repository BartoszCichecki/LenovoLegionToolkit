using System.Diagnostics;
using System.Reflection;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AboutPage
    {
        private string VersionText
        {
            get
            {
                var version = Assembly.GetEntryAssembly()?.GetName().Version;
                if (version == new System.Version(0, 0, 1, 0))
                    return "BETA";
                return version?.ToString(3) ?? "";
            }
        }
        private string BuildText => Assembly.GetEntryAssembly()?.GetBuildDateTime()?.ToString("yyyyMMddHHmmss") ?? "";

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

            _version.Text += $" {VersionText}";
            _build.Text += $" {BuildText}";
            _copyright.Text = CopyrightText;
        }
    }
}
