using System;
using System.Linq;
using System.Windows;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using Microsoft.Win32;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class BootLogoWindow
{
    public BootLogoWindow()
    {
        InitializeComponent();

        Loaded += BootLogoWindow_Loaded;
    }

    private void BootLogoWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Refresh();
    }

    private void Refresh()
    {
        var (enabled, resolution, formats, _) = BootLogo.GetStatus();

        _descriptionTextBlock.Text = string.Format(Resource.BootLogoWindow_Description, resolution.DisplayName, string.Join(", ", formats.Select(f => f.ToString().ToUpper())));

        _defaultStatus.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        _customStatus.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        _customizeButton.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;
        _revertToDefaultButton.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void RevertToDefaultButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _revertToDefaultButton.IsEnabled = false;

            await BootLogo.DisableAsync();

            _resultTextBlock.Text = Resource.BootLogoWindow_SetDefaultSuccess;

            Refresh();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Default logo could not be set.", ex);
            _resultTextBlock.Text = string.Format(Resource.BootLogoWindow_SetDefaultFailed, GetDescription(ex));
        }
        finally
        {
            _revertToDefaultButton.IsEnabled = true;
        }
    }

    private async void CustomizeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _customizeButton.IsEnabled = false;

            var (_, _, _, filters) = BootLogo.GetStatus();

            var ofd = new OpenFileDialog
            {
                Title = "Open",
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
                Filter = $"Images|{string.Join(";", filters)}",
                CheckFileExists = true,
            };

            var result = ofd.ShowDialog() ?? false;
            if (!result)
                return;

            var file = ofd.FileName;

            await BootLogo.EnableAsync(file);

            _resultTextBlock.Text = Resource.BootLogoWindow_SetCustomSuccess;

            Refresh();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Custom logo could not be set.", ex);

            _resultTextBlock.Text = string.Format(Resource.BootLogoWindow_SetCustomFailed, GetDescription(ex));
        }
        finally
        {
            _customizeButton.IsEnabled = true;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private static string GetDescription(Exception exception) => exception switch
    {
        CantSetUEFIPrivilegeException => Resource.BootLogoWindow_SetError_Cannot_Set_UEFI_Privilege,
        CantMountUEFIPartitionException => Resource.BootLogoWindow_SetError_Cannot_Mount_EFI_Partition,
        NotEnoughSpaceOnUEFIPartitionException => Resource.BootLogoWindow_SetError_Not_Enough_Free_Space_On_EFI_Partition,
        InvalidBootLogoImageSizeException => Resource.BootLogoWindow_SetError_Invalid_Image_Size,
        InvalidBootLogoImageFormatException => Resource.BootLogoWindow_SetError_Invalid_Image_Format,
        _ => exception.Message
    };
}
