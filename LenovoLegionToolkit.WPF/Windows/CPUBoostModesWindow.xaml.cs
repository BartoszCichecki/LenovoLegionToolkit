using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.WPF.Controls;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class CPUBoostModesWindow : Window
    {
        private readonly CPUBoostModeController _cpuBoostController = Container.Resolve<CPUBoostModeController>();

        public CPUBoostModesWindow()
        {
            InitializeComponent();

            Loaded += CPUBoostModesWindow_Loaded;
            IsVisibleChanged += CPUBoostModesWindow_IsVisibleChanged;
        }

        private async void CPUBoostModesWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async void CPUBoostModesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var settings = await _cpuBoostController.GetSettingsAsync();

            _stackPanel.Children.Clear();
            foreach (var setting in settings)
                _stackPanel.Children.Add(new CPUBoostModeControl(setting));
        }
    }
}
