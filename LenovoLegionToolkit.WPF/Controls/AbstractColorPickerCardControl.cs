using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Common;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractColorPickerCardControl<T> : AbstractRefreshingControl where T : struct
    {
        private readonly IFeature<T> _feature = Container.Resolve<IFeature<T>>();


        private readonly CardControl _cardControl = new();
        private readonly Button _pick = new();

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



        public AbstractColorPickerCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            _pick.Click += Pick_Click;
            _pick.Visibility = Visibility.Hidden;
            _pick.Width=64;
            _cardControl.Margin = new Thickness(0, 0, 0, 8);
            _cardControl.Content = _pick;

            Content = _cardControl;
        }

        private async void Pick_Click(object sender, RoutedEventArgs e) => await OnStateChange(_pick, _feature);

        //protected override async Task OnRefreshAsync() => _pick.Background = OnState.Equals(await _feature.GetStateAsync());
        protected override async Task OnRefreshAsync() => _pick.Visibility= Visibility.Visible;
        protected override void OnFinishedLoading() => _pick.Visibility = Visibility.Visible;

        protected virtual async Task OnStateChange(Button toggle, IFeature<T> feature)
        {
            //await feature.SetStateAsync();
        }
    }
}
