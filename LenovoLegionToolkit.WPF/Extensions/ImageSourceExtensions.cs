using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LenovoLegionToolkit.WPF.Extensions
{
    public static class ImageSourceExtensions
    {
        public static ImageSource FromResource(string name)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var uri = new Uri("pack://application:,,,/" + assemblyName + ";component/" + name, UriKind.RelativeOrAbsolute);
            return BitmapFrame.Create(uri);
        }
    }
}
