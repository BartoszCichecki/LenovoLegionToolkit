using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows.Settings
{
    public partial class ExcludeRefreshRatesWindow
    {
        private readonly RefreshRateFeature _feature = IoCContainer.Resolve<RefreshRateFeature>();
        private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

        public ExcludeRefreshRatesWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += PickProcessesWindow_Loaded;
            IsVisibleChanged += PickProcessesWindow_IsVisibleChanged;
        }

        private async void PickProcessesWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void PickProcessesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            _loader.IsLoading = true;

            var refreshRates = await _feature.GetAllStatesAsync();
            var excluded = _settings.Store.ExcludedRefreshRates;

            if (refreshRates.IsEmpty())
            {
                await Task.Delay(500);

                var result = await MessageBoxHelper.ShowAsync(this,
                    "No Refresh Rates found",
                    "Make sure that laptop display is on. Lenovo Legion Toolkit can't load refresh rates for a display that is not on.",
                    "Try again",
                    "Cancel");

                if (result)
                    await RefreshAsync();
                else
                    Close();

                return;
            }

            _list.Items.Clear();
            foreach (var refreshRate in refreshRates.OrderBy(rr => rr.Frequency))
            {
                var item = new ListItem(refreshRate)
                {
                    IsChecked = !excluded.Contains(refreshRate)
                };
                _list.Items.Add(item);
            }

            _loader.IsLoading = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var excludedRefreshRates = _list.Items.OfType<ListItem>()
                .Where(li => !li.IsChecked)
                .Select(li => li.RefreshRate)
                .ToArray();

            _settings.Store.ExcludedRefreshRates.Clear();
            _settings.Store.ExcludedRefreshRates.AddRange(excludedRefreshRates);
            _settings.SynchronizeStore();

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private class ListItem : UserControl
        {
            private readonly Grid _grid = new()
            {
                Margin = new(8, 4, 0, 16),
                ColumnDefinitions =
                {
                    new() { Width = new(32, GridUnitType.Pixel) },
                    new() { Width = new(1, GridUnitType.Star) },
                },
            };

            private readonly CheckBox _checkBox = new();

            private readonly TextBlock _nameTextBox = new()
            {
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            public RefreshRate RefreshRate { get; }

            public bool IsChecked
            {
                get => _checkBox.IsChecked ?? false;
                set => _checkBox.IsChecked = value;
            }

            public ListItem(RefreshRate refreshRate)
            {
                RefreshRate = refreshRate;

                InitializeComponent();
            }

            private void InitializeComponent()
            {
                _nameTextBox.Text = RefreshRate.DisplayName;

                Grid.SetColumn(_checkBox, 0);
                Grid.SetColumn(_nameTextBox, 1);

                _grid.Children.Add(_checkBox);
                _grid.Children.Add(_nameTextBox);

                Content = _grid;
            }
        }
    }
}
