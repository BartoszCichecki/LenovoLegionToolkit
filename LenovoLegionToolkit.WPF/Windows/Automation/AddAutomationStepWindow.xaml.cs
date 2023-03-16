using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Controls.Automation;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Automation;

public partial class AddAutomationStepWindow
{
    private readonly AbstractAutomationStepControl[] _controls;
    private readonly Action<AbstractAutomationStepControl> _addStepControl;

    public AddAutomationStepWindow(AbstractAutomationStepControl[] controls, Action<AbstractAutomationStepControl> addStepControl)
    {
        _controls = controls;
        _addStepControl = addStepControl;

        InitializeComponent();
    }

    private async void AddAutomationStepWindow_Loaded(object _1, RoutedEventArgs _2) => await RefreshAsync();

    private async void AddAutomationStepWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
        if (IsLoaded && IsVisible)
            await RefreshAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    private Task RefreshAsync()
    {
        _content.Children.Clear();

        foreach (var control in _controls)
            _content.Children.Add(CreateCardControl(control));

        return Task.CompletedTask;
    }

    private UIElement CreateCardControl(AbstractAutomationStepControl stepControl)
    {
        var control = new CardControl
        {
            Icon = stepControl.Icon,
            Header = new CardHeaderControl
            {
                Title = stepControl.Title,
                Accessory = new SymbolIcon { Symbol = SymbolRegular.ChevronRight24 }
            },
            Margin = new(0, 8, 0, 0),
        };

        control.Click += (_, _) =>
        {
            _addStepControl(stepControl);
            Close();
        };

        return control;
    }
}
