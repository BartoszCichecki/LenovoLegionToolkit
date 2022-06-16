using System;
using System.Windows;
using System.Windows.Controls;
using WPFUI.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Input;
using Point = System.Windows.Point;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;



namespace LenovoLegionToolkit.WPF.Controls
{
    public class ColorTable : Panel
    {

        public event EventHandler OnColorChange;

        protected virtual void ColorChangedEvent(EventArgs e)
        {
            EventHandler handler = OnColorChange;
            handler?.Invoke(this, e);
        }

        private double _value = 1;
        private Point current_color_point = new Point(128, 128);

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public System.Windows.Media.Color color   // property
        {
            get
            {
                return ColorFromXY(current_color_point);
            }
             set {
                current_color_point = ColorToXY(value);
                InvalidateVisual();
             } 
        }

        public double Value   // property
        {
            get { return _value; }   // get method
            set { _value = value; InvalidateVisual(); }  // set method
        }



        protected override void OnMouseMove(MouseEventArgs e)
        {
            
          if (e.RightButton == MouseButtonState.Pressed)
            {
                base.OnMouseMove(e);
                Point point_abs = e.GetPosition(this);
                Point point_rel = e.GetPosition(this);
                point_rel.X = point_abs.X - 128;
                point_rel.Y = point_abs.Y - 128;
                if ((point_rel.X * point_rel.X + point_rel.Y * point_rel.Y) > 127 * 127)
                { 
                    double angle = Math.Atan2(point_rel.Y, point_rel.X);
                    point_abs.X = 127 * Math.Cos(angle)+128;
                    point_abs.Y = 127 * Math.Sin(angle) + 128;
                }

                current_color_point = point_abs;
                ColorChangedEvent(e);
                InvalidateVisual();
            }
        }


        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            Bitmap bitmap = new Bitmap(256, 256);
            ColorFromHSV(0, 1, 1);
            for ( int x = -128; x<128; x++)
            {
                int ymax = (int)Math.Floor(Math.Sqrt(128 * 128 - x * x));
                for (int y = -ymax; y < ymax; y++)
                {
                    double angle = Math.Atan2(y, x)*180/Math.PI;
                    double radius_norm = Math.Sqrt(y*y + x * x) / 128;
                    bitmap.SetPixel(x+128,y+128, ColorFromHSV(angle+180, radius_norm, Value));
                }
            }
            SolidColorBrush mainBrush = new SolidColorBrush();
            mainBrush.Color = System.Windows.Media.Colors.Transparent;

            System.Windows.Media.Pen myPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 2);

            dc.DrawImage(Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions()), new Rect(0, 0, 256, 256));
            dc.DrawEllipse(mainBrush, myPen, new Point(128, 128), 128, 128);

            SolidColorBrush currentColorBrush = new SolidColorBrush();
            currentColorBrush.Color = ColorFromXY(current_color_point);
            System.Windows.Media.Pen myColorPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 4);
            dc.DrawEllipse(currentColorBrush, myColorPen, current_color_point, 15, 15);

        }

        private System.Windows.Media.Color ColorFromXY(Point cord)
        {
            Point point = new Point(cord.X - 128, cord.Y - 128);
            double radius_norm = Math.Sqrt(point.X*point.X + point.Y * point.Y) / 128;
            double angle = Math.Atan2(point.Y, point.X) * 180 / Math.PI;
            Color color = ColorFromHSV(angle + 180, radius_norm, Value);

            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        private Point ColorToXY(System.Windows.Media.Color color)
        {
            double h, s, v;
            ColorToHSV(System.Drawing.Color.FromArgb(color.R, color.G, color.B),out h,out s,out v);
            _value = v;
            Point point = new Point(128-s * 128 * Math.Cos(h * Math.PI / 180), 128-s * 128 * Math.Sin(h * Math.PI / 180));
            return point;
        }


        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

    }
}
