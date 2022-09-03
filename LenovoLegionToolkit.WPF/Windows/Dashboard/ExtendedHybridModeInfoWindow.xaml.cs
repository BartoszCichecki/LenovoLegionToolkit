using System.Windows;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard
{
    public partial class ExtendedHybridModeInfoWindow
    {
        public ExtendedHybridModeInfoWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
