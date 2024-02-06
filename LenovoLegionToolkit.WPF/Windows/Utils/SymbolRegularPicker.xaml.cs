using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Windows.Utils;

public partial class SymbolRegularPicker
{
    private readonly ThrottleLastDispatcher _throttleDispatcher = new(TimeSpan.FromMilliseconds(500));

    private readonly TaskCompletionSource<SymbolRegular?> _tcs = new();

    public Task<SymbolRegular?> SymbolRegularTask => _tcs.Task;

    public SymbolRegularPicker()
    {
        InitializeComponent();
    }

    private void SymbolRegularPicker_Loaded(object sender, RoutedEventArgs e) => Refresh();

    private void SymbolRegularPicker_Closing(object? sender, CancelEventArgs e) => _tcs.TrySetCanceled();

    private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e) => await _throttleDispatcher.DispatchAsync(() =>
    {
        Dispatcher.Invoke(Refresh);
        return Task.CompletedTask;
    });

    private void ItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Icon: SymbolIcon icon })
            return;

        _tcs.TrySetResult(icon.Symbol);
        Close();
    }

    private void DefaultButton_Click(object sender, RoutedEventArgs e)
    {
        _tcs.TrySetResult(null);
        Close();
    }

    private void Refresh()
    {
        _itemsControl.Items.Clear();

        var items = Enum.GetNames<SymbolRegular>()
                .Where(s => s.EndsWith("24", StringComparison.CurrentCultureIgnoreCase))
                .Where(s => s.Contains(_filterTextBox.Text, StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(s => s)
                .ToArray();

        foreach (var item in items)
        {
            var button = new Button()
            {
                Icon = Enum.Parse<SymbolRegular>(item).GetIcon(),
                FontSize = 32,
                Width = 80,
                Height = 80,
                Margin = new(0, 0, 4, 4)
            };
            button.Click += ItemButton_Click;
            _itemsControl.Items.Add(button);
        }
    }
}
