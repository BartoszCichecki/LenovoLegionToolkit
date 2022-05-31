using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using WPFUI.Common;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractToggleCardControl<T> : AbstractRefreshingControl where T : struct
    {
        private readonly IFeature<T> _feature = DIContainer.Resolve<IFeature<T>>();

        private readonly CardControl _cardControl = new();
        private readonly ToggleSwitch _toggle = new();

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

        protected abstract T OnState { get; }

        protected abstract T OffState { get; }

        public AbstractToggleCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            _toggle.Click += Toggle_Click;
            _toggle.Visibility = Visibility.Hidden;

            _cardControl.Margin = new(0, 0, 0, 8);
            _cardControl.Content = _toggle;

            Content = _cardControl;
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e) => await OnStateChange(_toggle, _feature);

        protected override async Task OnRefreshAsync() => _toggle.IsChecked = OnState.Equals(await _feature.GetStateAsync());

        protected override void OnFinishedLoading() => _toggle.Visibility = Visibility.Visible;

        protected virtual async Task OnStateChange(ToggleSwitch toggle, IFeature<T> feature)
        {
            if (IsRefreshing || toggle.IsChecked is null)
                return;

            var state = toggle.IsChecked.Value ? OnState : OffState;
            if (state.Equals(await feature.GetStateAsync()))
                return;

            await feature.SetStateAsync(state);
        }
    }
}
