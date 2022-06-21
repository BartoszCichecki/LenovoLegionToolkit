using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public abstract class AbstractRadioButtonDashboardCardControl<T> : AbstractRefreshingDashboardControl where T : struct
    {
        private readonly string _radioGroupName = Guid.NewGuid().ToString();

        private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

        private readonly CardControl _cardControl = new();

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly StackPanel _radioContainer = new();

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

        public AbstractRadioButtonDashboardCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            _radioContainer.Width = 150;
            _radioContainer.HorizontalAlignment = HorizontalAlignment.Left;
            _radioContainer.Visibility = Visibility.Hidden;

            _cardControl.Margin = new(0, 0, 0, 8);

            _cardControl.Header = _cardHeaderControl;
            _cardControl.Content = _radioContainer;

            Content = _cardControl;
        }

        protected override async Task OnRefreshAsync()
        {
            var items = await _feature.GetAllStatesAsync();
            var selectedItem = await _feature.GetStateAsync();

            static string displayName(T value)
            {
                if (value is IDisplayName dn)
                    return dn.DisplayName;
                if (value is Enum e)
                    return e.GetDisplayName();
                return value.ToString() ?? throw new InvalidOperationException("Unsupported type");
            }

            _radioContainer.Children.Clear();
            foreach (var item in items)
            {
                var radioButton = new RadioButton()
                {
                    GroupName = _radioGroupName,
                    Content = displayName(item),
                    Tag = item,
                    IsChecked = item.Equals(selectedItem),
                };
                radioButton.Checked += RadioButton_Checked;
                _radioContainer.Children.Add(radioButton);
            }
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            await OnStateChange((RadioButton)sender, _feature);
        }

        protected override void OnFinishedLoading()
        {
            _radioContainer.Visibility = Visibility.Visible;

            MessagingCenter.Subscribe<T>(this, () => Dispatcher.InvokeTask(RefreshAsync));
        }

        protected virtual async Task OnStateChange(RadioButton radioButton, IFeature<T> feature)
        {
            if (IsRefreshing)
                return;

            var selectedState = (T)radioButton.Tag;

            T currentState = await feature.GetStateAsync();

            if (selectedState.Equals(currentState))
                return;

            await feature.SetStateAsync(selectedState);
        }
    }
}
