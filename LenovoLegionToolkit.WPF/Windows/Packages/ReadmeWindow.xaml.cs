using System.Windows;

namespace LenovoLegionToolkit.WPF.Windows.Packages
{
    public partial class ReadmeWindow
    {
        public ReadmeWindow(string readme)
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            _content.Text = readme;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
