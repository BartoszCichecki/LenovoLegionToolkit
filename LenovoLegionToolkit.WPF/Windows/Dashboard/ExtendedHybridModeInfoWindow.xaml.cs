using System.Windows;

namespace LenovoLegionToolkit.WPF.Windows.Dashboard;

public partial class ExtendedHybridModeInfoWindow
{
    public ExtendedHybridModeInfoWindow() => InitializeComponent();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}