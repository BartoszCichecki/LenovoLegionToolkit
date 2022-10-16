using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common;
using System.Threading.Tasks;
using LenovoLegionToolkit.WPF.Windows.Automation;
using System;
using LenovoLegionToolkit.Lib.Extensions;
using System.Linq;

namespace LenovoLegionToolkit.WPF.Controls.Automation.Steps
{
    public class ProcessAutomationStepControl : AbstractAutomationStepControl<ProcessAutomationStep>
    {
        private readonly Button _buttonProcesses = new ()
        {
            Content = "Processes",
            MinWidth = 150,
        };

        private readonly ComboBox _comboBoxState = new()
        {
            MinWidth = 150,
            Visibility = Visibility.Hidden,
        };

        private readonly StackPanel _stackPanel = new();

        private ProcessAutomationState _state;

        public ProcessAutomationStepControl(ProcessAutomationStep step) : base(step)
        {
            Icon = SymbolRegular.Apps24;
            Title = "Processes Automation";
            Subtitle = "Select Processes to start or stop.";
        }

        public override IAutomationStep CreateAutomationStep() => new ProcessAutomationStep(_state);

        protected override UIElement? GetCustomControl()
        {
            _buttonProcesses.Click += (s, e) =>
            {
                ProcessInfo[] processes;
                if (AutomationStep.State.Processes != null)
                    processes = AutomationStep.State.Processes;
                else
                    processes = new ProcessInfo[] { };

                var window = new PickProcessesWindow(processes)
                {
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ShowInTaskbar = false,
                };
                window.OnSave += (s, e) =>
                {
                    _state.Processes = e;
                    RaiseChanged();
                };
                window.ShowDialog();
            };

            _stackPanel.Children.Add(_buttonProcesses);

            _comboBoxState.SelectionChanged += ComboBox_SelectionChanged;

            _stackPanel.Children.Add(_comboBoxState);

            return _stackPanel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_comboBoxState.TryGetSelectedItem(out ProcessState selectedState) || _state.Equals(selectedState))
                return;

            _state.State = selectedState;

            RaiseChanged();
        }

        protected override void OnFinishedLoading() => _comboBoxState.Visibility = Visibility.Visible;

        protected override Task RefreshAsync()
        {
            var items = Task.FromResult(Enum.GetValues<ProcessState>());
            var selectedItem = AutomationStep.State.State;

            static string displayName(ProcessState value)
            {
                if (value is Enum e)
                    return e.GetDisplayName();
                return value.ToString() ?? throw new InvalidOperationException("Unsupported type");
            }

            _state.State = selectedItem;
            _comboBoxState.SetItems(items.Result, selectedItem, displayName);
            _comboBoxState.IsEnabled = items.Result.Any();

            return Task.CompletedTask;
        }
    }
}
