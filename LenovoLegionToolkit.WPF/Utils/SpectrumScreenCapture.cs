using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;

namespace LenovoLegionToolkit.WPF.Utils
{
    public class SpectrumScreenCapture : SpectrumKeyboardBacklightController.IScreenCapture
    {
        private const PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppRgb;

        public void CaptureScreen(ref RGBColor[,] buffer, int width, int height, CancellationToken token)
        {
            var screen = Screen.PrimaryScreen.Bounds;

            using var targetImage = new Bitmap(width, height, PIXEL_FORMAT);

            using (var image = new Bitmap(screen.Width, screen.Height, PIXEL_FORMAT))
            {
                using (var graphics = Graphics.FromImage(image))
                    graphics.CopyFromScreen(screen.Left, screen.Top, 0, 0, screen.Size);

                token.ThrowIfCancellationRequested();

                using var targetGraphics = Graphics.FromImage(targetImage);
                targetGraphics.DrawImage(image, new Rectangle(0, 0, width, height));
            }

            token.ThrowIfCancellationRequested();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pixel = targetImage.GetPixel(x, y);
                    buffer[x, y] = new RGBColor(pixel.R, pixel.G, pixel.B);

                    token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
