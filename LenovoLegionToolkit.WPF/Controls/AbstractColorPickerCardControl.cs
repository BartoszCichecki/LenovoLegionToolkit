using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using WPFUI.Common;
using WPFUI.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractColorPickerCardControl<T> : AbstractRefreshingControl where T : struct
    {
        private readonly IFeature<T> _feature = Container.Resolve<IFeature<T>>();


        private readonly CardControl _cardControl = new();
        private readonly ColorPicker _colorPicker = new ColorPicker();
        private readonly Button      _color = new Button();
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
            _color.Width = 200;
            _color.IsEnabled = false;
            _cardControl.Click += Card_click;
            _cardControl.Margin = new Thickness(0, 0, 0, 8);
            _cardControl.Content = _color;
            _colorPicker.OnColorChange += OnColorChange;
            _colorPicker.Margin = new Thickness(0, 0, 0, 8);
            _colorPicker.HorizontalAlignment = HorizontalAlignment.Center;
            Content = _cardControl;


        }


        private void OnColorChange(object? sender, EventArgs e)
        {
            if (_cardControl.Title == "Zone1")
            {
                KeyboardData.LegionRGBKey.ZONE1_RGB[0] = _colorPicker.color.R;
                KeyboardData.LegionRGBKey.ZONE1_RGB[1] = _colorPicker.color.G;
                KeyboardData.LegionRGBKey.ZONE1_RGB[2] = _colorPicker.color.B;
            }
            if (_cardControl.Title == "Zone2")
            {
                KeyboardData.LegionRGBKey.ZONE2_RGB[0] = _colorPicker.color.R;
                KeyboardData.LegionRGBKey.ZONE2_RGB[1] = _colorPicker.color.G;
                KeyboardData.LegionRGBKey.ZONE2_RGB[2] = _colorPicker.color.B;
            }
            if (_cardControl.Title == "Zone3")
            {
                KeyboardData.LegionRGBKey.ZONE3_RGB[0] = _colorPicker.color.R;
                KeyboardData.LegionRGBKey.ZONE3_RGB[1] = _colorPicker.color.G;
                KeyboardData.LegionRGBKey.ZONE3_RGB[2] = _colorPicker.color.B;
            }
            if (_cardControl.Title == "Zone4")
            {
                KeyboardData.LegionRGBKey.ZONE4_RGB[0] = _colorPicker.color.R;
                KeyboardData.LegionRGBKey.ZONE4_RGB[1] = _colorPicker.color.G;
                KeyboardData.LegionRGBKey.ZONE4_RGB[2] = _colorPicker.color.B;
            }
            OnStateChange(_feature);
        }

        private void Card_click(object sender, RoutedEventArgs e)
        {
            if (_cardControl.Content == _color)
            {
                _cardControl.Content = _colorPicker;
            }
            else
            {
                _cardControl.Content = _color;
            }
        }



        //protected override async Task OnRefreshAsync() => _pick.Background = OnState.Equals(await _feature.GetStateAsync());
        protected override async Task OnRefreshAsync() => _cardControl.Visibility= Visibility.Visible;
        protected override void OnFinishedLoading()
        {
            Color color;
            if (_cardControl.Title == "Zone1")
            {
                color = Color.FromRgb(KeyboardData.LegionRGBKey.ZONE1_RGB[0], KeyboardData.LegionRGBKey.ZONE1_RGB[1], KeyboardData.LegionRGBKey.ZONE1_RGB[2]);

            }
            if (_cardControl.Title == "Zone2")
            {
                color = Color.FromRgb(KeyboardData.LegionRGBKey.ZONE2_RGB[0], KeyboardData.LegionRGBKey.ZONE2_RGB[1], KeyboardData.LegionRGBKey.ZONE2_RGB[2]);
            }
            if (_cardControl.Title == "Zone3")
            {
                color = Color.FromRgb(KeyboardData.LegionRGBKey.ZONE3_RGB[0], KeyboardData.LegionRGBKey.ZONE3_RGB[1], KeyboardData.LegionRGBKey.ZONE3_RGB[2]);
            }
            if (_cardControl.Title == "Zone4")
            {
                color = Color.FromRgb(KeyboardData.LegionRGBKey.ZONE4_RGB[0], KeyboardData.LegionRGBKey.ZONE4_RGB[1], KeyboardData.LegionRGBKey.ZONE4_RGB[2]);
            }
            _colorPicker.color = color;
            SolidColorBrush currentColorBrush = new SolidColorBrush();
            currentColorBrush.Color = _colorPicker.color;
            _color.Background = currentColorBrush;
            OnStateChange(_feature);
        }

        protected virtual async Task OnStateChange( IFeature<T> feature)
        {
            T currentState = await feature.GetStateAsync();
            await feature.SetStateAsync(currentState);
        }
    }
}
