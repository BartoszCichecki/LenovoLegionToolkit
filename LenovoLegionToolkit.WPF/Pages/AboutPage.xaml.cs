using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Pages;

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

        _translationCredit.Visibility = Resource.Culture.Equals(new CultureInfo("en")) ? Visibility.Collapsed : Visibility.Visible;
    }

    private void OpenApplicationDataFolder_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer", Folders.AppData);
    }

    private void OpenApplicationTempFolder_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer", Folders.Temp);
    }
}