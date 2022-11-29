using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.Automation;

public class AbstractComboBoxAutomationStepCardControl<T> : AbstractAutomationStepControl<IAutomationStep<T>> where T : struct
{

    private readonly ComboBox _comboBox = new()
    {
        MinWidth = 150,
        Visibility = Visibility.Hidden,
    };

    private T _state;

    public AbstractComboBoxAutomationStepCardControl(IAutomationStep<T> step) : base(step) { }

    protected override UIElement? GetCustomControl()
    {
        _comboBox.SelectionChanged += ComboBox_SelectionChanged;

        return _comboBox;
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_comboBox.TryGetSelectedItem(out T selectedState) || _state.Equals(selectedState))
            return;

        _state = selectedState;

        RaiseChanged();
    }

    public override IAutomationStep CreateAutomationStep()
    {
        var obj = Activator.CreateInstance(AutomationStep.GetType(), _state);
        if (obj is not IAutomationStep<T> step)
            throw new InvalidOperationException("Couldn't create automation step.");
        return step;
    }

    protected virtual string ComboBoxItemDisplayName(T value) => value switch
    {
        IDisplayName dn => dn.DisplayName,
        Enum e => e.GetDisplayName(),
        _ => value.ToString() ?? throw new InvalidOperationException("Unsupported type")
    };

    protected override async Task RefreshAsync()
    {
        var items = await AutomationStep.GetAllStatesAsync();
        var selectedItem = AutomationStep.State;

        _state = selectedItem;
        _comboBox.SetItems(items, selectedItem, ComboBoxItemDisplayName);
        _comboBox.IsEnabled = items.Any();
    }

    protected override void OnFinishedLoading() => _comboBox.Visibility = Visibility.Visible;
}