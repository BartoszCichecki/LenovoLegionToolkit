using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib.Automation.Steps;
using WPFUI.Common;
using WPFUI.Controls;
using Button = WPFUI.Controls.Button;

namespace LenovoLegionToolkit.WPF.Controls.Automation
{
    public abstract class AbstractAutomationStepControl : UserControl
    {
        private readonly CardControl _cardControl = new()
        {
            Margin = new(0, 0, 0, 8),
        };

        private readonly StackPanel _stackPanel = new()
        {
            Orientation = Orientation.Horizontal,
        };

        private readonly Button _deleteButton = new()
        {
            Icon = SymbolRegular.Dismiss24,
            Appearance = Appearance.Secondary,
            Width = 34,
            Height = 34,
            Margin = new(8, 0, 0, 0),
        };

        public abstract IAutomationStep AutomationStep { get; }

        protected abstract UIElement? CustomControl { get; }

        public SymbolRegular Icon
        {
            get => _cardControl.Icon;
            set => _cardControl.Icon = value;
        }

        public string Title
        {
            get => _cardControl.Title;
            set => _cardControl.Title = value;
        }

        public string Subtitle
        {
            get => _cardControl.Subtitle;
            set => _cardControl.Subtitle = value;
        }

        public event EventHandler? OnDelete;

        public AbstractAutomationStepControl()
        {
            InitializeComponent();

            Loaded += RefreshingControl_Loaded;
            IsVisibleChanged += RefreshingControl_IsVisibleChanged;
        }

        private void InitializeComponent()
        {
            _deleteButton.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);

            if (CustomControl != null)
                _stackPanel.Children.Add(CustomControl);
            _stackPanel.Children.Add(_deleteButton);

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

        protected abstract void OnFinishedLoading();

        protected abstract Task RefreshAsync();
    }
}
