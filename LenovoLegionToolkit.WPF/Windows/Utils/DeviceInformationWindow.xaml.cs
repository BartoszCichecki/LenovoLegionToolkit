using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Utils
{
    public partial class DeviceInformationWindow
    {
        private readonly WarrantyChecker _warrantyChecker = IoCContainer.Resolve<WarrantyChecker>();

        public DeviceInformationWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.NoResize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += DeviceInformationWindow_Loaded;
        }

        private async void DeviceInformationWindow_Loaded(object sender, RoutedEventArgs e) => await RefreshAsync();

        private async Task RefreshAsync(bool forceRefresh = false)
        {
            var mi = await Compatibility.GetMachineInformation();

            _manufacturerLabel.Content = mi.Vendor;
            _modelLabel.Content = mi.Model;
            _mtmLabel.Content = mi.MachineType;
            _serialNumberLabel.Content = mi.SerialNumber;
            _biosLabel.Content = mi.BIOSVersion;

            try
            {
                _refreshWarrantyButton.IsEnabled = false;

                _warrantyStatusLabel.Content = "-";
                _warrantyStartLabel.Content = "-";
                _warrantyEndLabel.Content = "-";
                _warrantyLinkCardAction.Tag = null;
                _warrantyLinkCardAction.IsEnabled = false;

                var warrantyInfo = await _warrantyChecker.GetWarrantyInfo(mi, forceRefresh);
                _warrantyLinkCardAction.IsEnabled = false;

                _warrantyStatusLabel.Content = warrantyInfo.Status ?? "-";
                _warrantyStartLabel.Content = warrantyInfo.Start is not null ? $"{warrantyInfo.Start:d}" : "-";
                _warrantyEndLabel.Content = warrantyInfo.End is not null ? $"{warrantyInfo.End:d}" : "-";
                _warrantyLinkCardAction.Tag = warrantyInfo.Link;
                _warrantyLinkCardAction.IsEnabled = true;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't load warranty info.", ex);
            }
            finally
            {
                _refreshWarrantyButton.IsEnabled = true;
            }
        }

        private async void RefreshWarrantyButton_OnClick(object sender, RoutedEventArgs e) => await RefreshAsync(true);

        private async void DeviceCardControl_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as CardControl)?.Content as Label)?.Content is not string str)
                return;

            try
            {
                Clipboard.SetText(str);
                await _snackBar.ShowAsync("Copied!", $"\"{str}\" copied to clipboard.");
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't copy to clipboard", ex);
            }
        }

        private void WarrantyLinkCardAction_OnClick(object sender, RoutedEventArgs e)
        {
            var link = _warrantyLinkCardAction.Tag as Uri;
            link?.Open();
        }
    }
}
