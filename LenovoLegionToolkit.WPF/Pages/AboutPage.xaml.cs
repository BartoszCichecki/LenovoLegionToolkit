using System.Reflection;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();

            _version.Text += Assembly.GetEntryAssembly().GetName().Version.ToString(3);
        }
    }
}
