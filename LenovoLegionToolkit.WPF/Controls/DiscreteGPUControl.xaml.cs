using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class DiscreteGPUControl
    {
        private readonly GPUController _gpuController = Container.Resolve<GPUController>();

        public DiscreteGPUControl()
        {
            InitializeComponent();

            _gpuController.Refreshed += GpuController_Refreshed;

            IsVisibleChanged += DiscreteGPUControl_IsVisibleChanged;
        }

        protected override async Task OnRefreshAsync()
        {
            if (!_gpuController.IsSupported())
                throw new InvalidOperationException("Unsupported operation");

            await _gpuController.StartAsync();
        }
        private async void DiscreteGPUControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                await _gpuController.StopAsync();
        }

        private void GpuController_Refreshed(object? sender, GPUController.RefreshedEventArgs e) => Dispatcher.Invoke(() =>
        {
            if (e.Status == GPUController.Status.Unknown || e.Status == GPUController.Status.NVIDIAGPUNotFound)
            {
                _discreteGPUStatusDescription.Text = "-";
                _discreteGPUStatusDescription.ToolTip = null;
                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Collapsed;
            }
            else if (e.IsActive)
            {
                var status = "Active";
                if (e.ProcessCount > 0)
                    status += $" ({e.ProcessCount} app{(e.ProcessCount > 1 ? "s" : "")})";
                _discreteGPUStatusDescription.Text = status;
                _discreteGPUStatusDescription.ToolTip = e.ProcessCount < 1 ? null : ("Processes:\n" + string.Join("\n", e.ProcessNames));
                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Visible;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Collapsed;
            }
            else
            {
                _discreteGPUStatusDescription.Text = "Inactive";
                _discreteGPUStatusDescription.ToolTip = "nVidia GPU is not active.";
                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Visible;
            }

            _deactivateGPUButton.IsEnabled = e.CanBeDeactivated;
            _deactivateGPUButton.ToolTip = e.Status switch
            {
                GPUController.Status.MonitorsConnected => "A monitor is connected to nVidia GPU.",
                GPUController.Status.DeactivatePossible => "nVidia GPU can be disabled.\n\nRemember, that some programs might crash if you do it.",
                GPUController.Status.Inactive => "nVidia GPU is not active.",
                _ => null,
            };
        });
    }
}
