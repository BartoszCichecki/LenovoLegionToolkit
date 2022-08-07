using System;
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

        private async void DeviceInformationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mi = await Compatibility.GetMachineInformation();

            _manufacturerLabel.Content = mi.Vendor;
            _modelLabel.Content = mi.Model;
            _mtmLabel.Content = mi.MachineType;
            _serialNumberLabel.Content = mi.SerialNumber;
            _biosLabel.Content = mi.BIOSVersion;

            try
            {
                var warrantyInfo = await _warrantyChecker.GetWarrantyInfo(mi, default);

                _warrantyStatusLabel.Content = warrantyInfo.Status ?? "-";
                _warrantyStartLabel.Content = warrantyInfo.Start is not null ? $"{warrantyInfo.Start:d}" : "-";
                _warrantyEndLabel.Content = warrantyInfo.End is not null ? $"{warrantyInfo.End:d}" : "-";

                if (warrantyInfo.Link is not null)
                {
                    _warrantyLinkCardAction.Click += (s, e) => warrantyInfo.Link.Open();
                    _warrantyLinkCardAction.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Couldn't load warranty info.", ex);
            }
        }

        private async void DeviceCardControl_Click(object sender, RoutedEventArgs e)
        {
            var str = ((sender as CardControl)?.Content as Label)?.Content as string;

            if (str is null)
                return;

            Clipboard.SetText(str);

            await _snackBar.ShowAsync("Copied!", $"\"{str}\" copied to clipboard.");
        }
    }
}
