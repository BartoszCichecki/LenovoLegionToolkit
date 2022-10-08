using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard
{
    public partial class DiscreteGPUControl
    {
        private readonly GPUController _gpuController = IoCContainer.Resolve<GPUController>();

        public DiscreteGPUControl()
        {
            InitializeComponent();

            _gpuController.Refreshed += GpuController_Refreshed;

            IsVisibleChanged += DiscreteGPUControl_IsVisibleChanged;
        }

        protected override void OnFinishedLoading() { }

        protected override async Task OnRefreshAsync()
        {
            if (!_gpuController.IsSupported())
                throw new InvalidOperationException("Unsupported operation");

            if (!IsVisible)
                return;

            await _gpuController.StartAsync();
        }
        private async void DiscreteGPUControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                return;

            _content.Visibility = Visibility.Hidden;

            await _gpuController.StopAsync();
        }

        private void GpuController_Refreshed(object? sender, GPUController.RefreshedEventArgs e) => Dispatcher.Invoke(() =>
        {
            var tooltipStringBuilder = new StringBuilder("Performance state:");
            tooltipStringBuilder.AppendLine().Append(e.PerformanceState ?? "Unknown").AppendLine().AppendLine();

            if (e.Status is GPUController.Status.Unknown or GPUController.Status.NVIDIAGPUNotFound)
            {
                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusDescription.Content = "-";
                _gpuInfoButton.ToolTip = null;
                _gpuInfoButton.IsEnabled = false;
                Visibility = Visibility.Collapsed;
                return;
            }

            if (e.IsActive)
            {
                var processesStringBuilder = new StringBuilder();

                if (e.ProcessCount > 0)
                {
                    processesStringBuilder.Append("Processes:");
                    foreach (var p in e.Processes)
                    {
                        try
                        {
                            processesStringBuilder.AppendLine().Append(" · ").Append(p.ProcessName);
                        }
                        catch { }
                    }
                }
                else
                {
                    processesStringBuilder.Append("No processes");
                }

                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Visible;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusDescription.Content = "Active";
                _gpuInfoButton.ToolTip = tooltipStringBuilder.Append(processesStringBuilder).ToString();
                _gpuInfoButton.IsEnabled = true;
            }
            else
            {
                _discreteGPUStatusActiveIndicator.Visibility = Visibility.Collapsed;
                _discreteGPUStatusInactiveIndicator.Visibility = Visibility.Visible;
                _discreteGPUStatusDescription.Content = "Inactive";
                _gpuInfoButton.ToolTip = tooltipStringBuilder.Append("nVidia GPU is not active.").ToString();
                _gpuInfoButton.IsEnabled = true;
            }

            _deactivateGPUButton.IsEnabled = e.CanBeDeactivated;

            if (e.CanBeDeactivated)
            {
                _deactivateGPUButtonText.SetResourceReference(ForegroundProperty, "TextOnAccentFillColorPrimaryBrush");
                _deactivateGPUButtonIcon.SetResourceReference(ForegroundProperty, "TextOnAccentFillColorPrimaryBrush");
            }
            else
            {
                _deactivateGPUButtonText.SetResourceReference(ForegroundProperty, "TextFillColorDisabledBrush");
                _deactivateGPUButtonIcon.SetResourceReference(ForegroundProperty, "TextFillColorDisabledBrush");
            }

            _deactivateGPUButton.ToolTip = e.Status switch
            {
                GPUController.Status.MonitorsConnected => "A monitor is connected to nVidia GPU.",
                GPUController.Status.DeactivatePossible => "nVidia GPU can be disabled.\n\nRemember, that some programs might crash if you do it.",
                GPUController.Status.Inactive => "nVidia GPU is not active.",
                _ => null,
            };

            _content.Visibility = Visibility.Visible;
        });

        private void DeactivateGPUButton_Click(object sender, RoutedEventArgs e)
        {
            if (_deactivateGPUButton.ContextMenu is null)
                return;

            _deactivateGPUButton.ContextMenu.PlacementTarget = _deactivateGPUButton;
            _deactivateGPUButton.ContextMenu.Placement = PlacementMode.Bottom;
            _deactivateGPUButton.ContextMenu.IsOpen = true;
        }

        private async void KillAppsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _deactivateGPUButton.IsEnabled = false;
            await _gpuController.KillGPUProcessesAsync();
        }

        private async void RestartGPUMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _deactivateGPUButton.IsEnabled = false;
            await _gpuController.DeactivateGPUAsync();
        }
    }
}
