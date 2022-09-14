using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class FanCurveControl
    {
        private Slider[] _sliders;

        public FanCurveControl()
        {
            InitializeComponent();

            _sliders = _slidersGrid.Children.OfType<Slider>().ToArray();
            DrawGraph();
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_sliders.Length < 2)
                return;

            if (sender is not Slider { IsMouseCaptureWithin: true } currentSlider)
                return;

            VerifyValues(currentSlider);
            DrawGraph();
        }

        private void VerifyValues(Slider currentSlider)
        {
            var currentIndex = Array.IndexOf(_sliders, currentSlider);
            if (currentIndex < 0)
                return;

            var currentValue = currentSlider.Value;
            var slidersBefore = _sliders[..currentIndex];
            var slidersAfter = _sliders[(currentIndex + 1)..];

            foreach (var slider in slidersBefore)
            {
                if (slider.Value > currentValue)
                    slider.Value = currentValue;
            }

            foreach (var slider in slidersAfter)
            {
                if (slider.Value < currentValue)
                    slider.Value = currentValue;
            }
        }

        private void DrawGraph()
        {
            var color = (SolidColorBrush)Application.Current.Resources["ControlFillColorDefaultBrush"];

            _canvas.Children.Clear();

            var points = _sliders
                .Select(GetThumbLocation)
                .Select(p => new Point(p.X, p.Y))
                .ToArray();

            // Line

            var pathSegmentCollection = new PathSegmentCollection();
            foreach (var point in points.Skip(1))
                pathSegmentCollection.Add(new LineSegment { Point = point });
            var pathFigure = new PathFigure { StartPoint = points[0], Segments = pathSegmentCollection };

            var path = new Path
            {
                StrokeThickness = 2,
                Stroke = color,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Data = new PathGeometry { Figures = new PathFigureCollection { pathFigure } },
            };
            _canvas.Children.Add(path);

            // Fill

            var pointCollection = new PointCollection();
            pointCollection.Add(new(points[0].X, _canvas.ActualHeight));
            foreach (var point in points)
                pointCollection.Add(point);
            pointCollection.Add(new(points[^1].X, _canvas.ActualHeight));

            var polygon = new Polygon
            {
                Fill = color,
                Points = pointCollection
            };
            _canvas.Children.Add(polygon);
        }

        private Point GetThumbLocation(Slider slider)
        {
            var ratio = slider.Value / (slider.Maximum - slider.Minimum);
            var y = slider.ActualHeight - (slider.ActualHeight * ratio);
            var x = slider.ActualWidth * 0.5;
            var point = slider.TranslatePoint(new(x, y), _canvas);
            return point;
        }
    }
}
