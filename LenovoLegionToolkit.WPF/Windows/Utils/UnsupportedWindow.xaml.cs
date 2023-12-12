using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Windows.Utils;

public partial class UnsupportedWindow
{
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ShouldContinue => _taskCompletionSource.Task;

    public UnsupportedWindow(MachineInformation mi)
    {
        InitializeComponent();

        _vendorText.Text = mi.Vendor;
        _modelText.Text = mi.Model;
        _machineTypeText.Text = mi.MachineType;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        for (var i = 5; i > 0; i--)
        {
            _continueButton.Content = $"Continue ({i})";
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        _continueButton.Content = "Continue";
        _continueButton.IsEnabled = true;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _taskCompletionSource.TrySetResult(false);
    }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
        _taskCompletionSource.TrySetResult(true);
        Close();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _taskCompletionSource.TrySetResult(false);
        Close();
    }
}
