using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.Automation
{
    public class AbstractComboBoxAutomationStepControl<T> : AbstractAutomationStepControl where T : struct
    {
        private readonly IAutomationStep<T> _step;

        private readonly ComboBox _comboBox = new()
        {
            Width = 150,
            Visibility = Visibility.Hidden,
        };

        public override IAutomationStep AutomationStep => _step;

        protected override UIElement? CustomControl => _comboBox;

        public AbstractComboBoxAutomationStepControl(IAutomationStep<T> step) => _step = step;

        protected override void OnFinishedLoading() => _comboBox.Visibility = Visibility.Visible;

        protected override async Task RefreshAsync()
        {
            var items = await _step.GetAllStatesAsync();
            var selectedItem = _step.State;

            static string displayName(T value)
            {
                if (value is IDisplayName dn)
                    return dn.DisplayName;
                if (value is Enum e)
                    return e.GetDisplayName();
                return value.ToString() ?? throw new InvalidOperationException("Unsupported type");
            }

            _comboBox.SetItems(items, selectedItem, displayName);
            _comboBox.IsEnabled = items.Any();
        }
    }
}
