using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class ImageSourceExtensions
    {
        public static ImageSource ApplicationIcon()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var path = "pack://application:,,,/" + assemblyName + ";component/Assets/icon.ico";
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(uri);
        }

        public static ImageSource? ApplicationIcon(string path)
        {
            try
            {
                var icon = Icon.ExtractAssociatedIcon(path);
                if (icon is null)
                    return null;

                var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                return imageSource;

            }
            catch
            {
                return null;
            }
        }
    }
}
