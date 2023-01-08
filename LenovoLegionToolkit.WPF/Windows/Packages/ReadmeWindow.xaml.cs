using System;
using System.Windows;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Packages;

public partial class ReadmeWindow
{
    public ReadmeWindow(string readme)
    {
        InitializeComponent();

        _content.Text = readme;
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(_content.Text);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't copy to clipboard", ex);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
