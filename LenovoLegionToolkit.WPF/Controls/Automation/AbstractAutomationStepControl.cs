using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Automation
{
    public abstract class AbstractAutomationStepControl<T> : AbstractAutomationStepControl where T : IAutomationStep
    {
        public new T AutomationStep => (T)base.AutomationStep;

        protected AbstractAutomationStepControl(T automationStep) : base(automationStep) { }
    }

    public abstract class AbstractAutomationStepControl : UserControl
    {
        public IAutomationStep AutomationStep { get; }

        private readonly CardControl _cardControl = new()
        {
            Margin = new(0, 0, 0, 8),
        };

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly StackPanel _stackPanel = new()
        {
            Orientation = Orientation.Horizontal,
        };

        private readonly Button _deleteButton = new()
        {
            Icon = SymbolRegular.Dismiss24,
            Width = 34,
            Height = 34,
            Margin = new(8, 0, 0, 0),
        };

        public SymbolRegular Icon
        {
            get => _cardControl.Icon;
            set => _cardControl.Icon = value;
        }

        public string Title
        {
            get => _cardHeaderControl.Title;
            set => _cardHeaderControl.Title = value;
        }

        public string Subtitle
        {
            get => _cardHeaderControl.Subtitle;
            set => _cardHeaderControl.Subtitle = value;
        }

        public event EventHandler? Changed;
        public event EventHandler? Delete;

        protected AbstractAutomationStepControl(IAutomationStep automationStep)
        {
            AutomationStep = automationStep;

            InitializeComponent();

            Loaded += RefreshingControl_Loaded;
            IsVisibleChanged += RefreshingControl_IsVisibleChanged;
        }

        private void InitializeComponent()
        {
            _deleteButton.Click += (s, e) => Delete?.Invoke(this, EventArgs.Empty);

            var control = GetCustomControl();
            if (control is not null)
                _stackPanel.Children.Add(control);
            _stackPanel.Children.Add(_deleteButton);

            _cardControl.Header = _cardHeaderControl;
            _cardControl.Content = _stackPanel;

            Content = _cardControl;
        }

        private async void RefreshingControl_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = Task.Delay(250);
            await RefreshAsync();
            await loadingTask;

            OnFinishedLoading();
        }

        private async void RefreshingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        public abstract IAutomationStep CreateAutomationStep();

        protected abstract UIElement? GetCustomControl();

        protected abstract void OnFinishedLoading();

        protected abstract Task RefreshAsync();

        protected void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
    }
}
