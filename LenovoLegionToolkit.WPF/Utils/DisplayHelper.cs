using System.Windows;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class DisplayHelper
    {
        public static Rect GetRealWorkArea()
        {
            var width = SystemParameters.PrimaryScreenWidth;
            var height = SystemParameters.PrimaryScreenHeight;
            var scale = GetRealScale();
            return new(0, 0, width * scale, height * scale);
        }

        private static double GetRealScale()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow is null)
                return 1;

            var target = PresentationSource.FromDependencyObject(mainWindow)?.CompositionTarget;
            if (target is null)
                return 1;

            var x = target.TransformToDevice.M11;
            var scale = 96.0 / (96.0 * x);
            return scale;
        }
    }
}
