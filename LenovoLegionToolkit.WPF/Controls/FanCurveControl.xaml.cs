using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Controls;

public partial class FanCurveControl
{
    private readonly List<Slider> _sliders = new();
    private readonly InfoTooltip _customToolTip = new();

    private FanTableData[]? _tableData;

    public FanCurveControl()
    {
        InitializeComponent();

        MouseLeave += FanCurveControl_MouseLeave;
    }

    private void FanCurveControl_MouseLeave(object sender, MouseEventArgs e)
    {
        _customToolTip.IsOpen = false;
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var size = base.ArrangeOverride(arrangeBounds);
        DrawGraph();
        return size;
    }

    public void SetFanTableInfo(FanTableInfo fanTableInfo)
    {
        _sliders.Clear();
        _slidersGrid.Children.Clear();

        var tableValues = fanTableInfo.Table.GetTable();

        for (var i = 0; i < tableValues.Length; i++)
        {
            var slider = GenerateSlider(i, 0, 10);
            slider.Value = tableValues[i];
            _sliders.Add(slider);
            _slidersGrid.Children.Add(slider);
        }

        _tableData = fanTableInfo.Data;

        Dispatcher.InvokeAsync(DrawGraph, DispatcherPriority.Render);
    }

    public FanTableInfo? GetFanTableInfo()
    {
        if (_tableData is null)
            return null;

        var fanTable = _sliders.Select(s => (ushort)s.Value).ToArray();
        return new(_tableData, new FanTable(fanTable));
    }

    private Slider GenerateSlider(int index, int minimum, int maximum)
    {
        var slider = new Slider
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Vertical,
            IsSnapToTickEnabled = true,
            TickFrequency = 1,
            Maximum = maximum,
            Minimum = minimum,
            Tag = index,
        };

        slider.MouseMove += Slider_MouseMove;
        slider.ValueChanged += Slider_OnValueChanged;

        Grid.SetColumn(slider, index + 1);

        return slider;
    }

    private void Slider_MouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not Slider slider)
            return;

        if (slider.Template.FindName("PART_Track", slider) is not Track track)
            return;

        if (!track.Thumb.IsMouseOver || _tableData is null)
        {
            _customToolTip.IsOpen = false;
            return;
        }

        _customToolTip.Update(_tableData, (int)slider.Tag, (int)slider.Value - 1);

        _customToolTip.Placement = PlacementMode.Custom;
        _customToolTip.PlacementTarget = track.Thumb;
        _customToolTip.CustomPopupPlacementCallback = ToolTipCustomPopupPlacementCallback;

        _customToolTip.HorizontalOffset += -0.1;
        _customToolTip.HorizontalOffset += +0.1;

        _customToolTip.IsOpen = true;
    }

    private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_sliders.Count < 10)
            return;

        if (sender is not Slider { IsMouseCaptureWithin: true } currentSlider)
            return;

        var index = (int)currentSlider.Tag;
        var minimum = FanTable.Minimum.GetTable();

        if (currentSlider.Value < minimum[index])
        {
            currentSlider.Value = minimum[index];
            return;
        }

        VerifyValues(currentSlider);
        DrawGraph();
    }

    private CustomPopupPlacement[] ToolTipCustomPopupPlacementCallback(Size size, Size targetSize, Point _)
    {
        return new CustomPopupPlacement[]
        {
            new(new((targetSize.Width - size.Width) * 0.5, -targetSize.Height -size.Height + 8), PopupPrimaryAxis.Vertical)
        };
    }

    private void VerifyValues(Slider currentSlider)
    {
        var currentIndex = _sliders.IndexOf(currentSlider);
        if (currentIndex < 0)
            return;

        var currentValue = currentSlider.Value;
        var slidersBefore = _sliders.Take(currentIndex);
        var slidersAfter = _sliders.Skip(currentIndex + 1);

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

        if (points.IsEmpty())
            return;

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

        var pointCollection = new PointCollection { new(points[0].X, _canvas.ActualHeight - 1) };
        foreach (var point in points)
            pointCollection.Add(point);
        pointCollection.Add(new(points[^1].X, _canvas.ActualHeight - 1));

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

    private class InfoTooltip : ToolTip
    {
        private readonly Grid _grid = new()
        {
            ColumnDefinitions =
            {
                new() { Width = GridLength.Auto},
                new () { Width = GridLength.Auto}
            },
            RowDefinitions =
            {
                new() { Height = GridLength.Auto},
                new() { Height = GridLength.Auto},
                new() { Height = GridLength.Auto}
            }
        };

        private readonly TextBlock _desc1 = new() { Text = Resource.FanCurveControl_CPU, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };
        private readonly TextBlock _desc2 = new() { Text = Resource.FanCurveControl_CPUSensor, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };
        private readonly TextBlock _desc3 = new() { Text = Resource.FanCurveControl_GPU, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };

        private readonly TextBlock _value1 = new();
        private readonly TextBlock _value2 = new();
        private readonly TextBlock _value3 = new();

        public InfoTooltip()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SetResourceReference(StyleProperty, typeof(ToolTip));

            Grid.SetColumn(_desc1, 0);
            Grid.SetColumn(_desc2, 0);
            Grid.SetColumn(_desc3, 0);
            Grid.SetColumn(_value1, 1);
            Grid.SetColumn(_value3, 1);
            Grid.SetColumn(_value2, 1);

            Grid.SetRow(_desc1, 0);
            Grid.SetRow(_desc2, 1);
            Grid.SetRow(_desc3, 2);
            Grid.SetRow(_value1, 0);
            Grid.SetRow(_value2, 1);
            Grid.SetRow(_value3, 2);

            _grid.Children.Add(_desc1);
            _grid.Children.Add(_desc2);
            _grid.Children.Add(_desc3);
            _grid.Children.Add(_value1);
            _grid.Children.Add(_value2);
            _grid.Children.Add(_value3);

            Content = _grid;
        }

        public void Update(FanTableData[] tableData, int index, int value)
        {
            try
            {
                _value1.Text = tableData
                    .Where(td => td.Type == FanTableType.CPU)
                    .Select(td => GetDescription(td, index, value))
                    .FirstOrDefault() ?? "-";
            }
            catch
            {
                _value1.Text = "-";
            }

            try
            {
                _value2.Text = tableData
                    .Where(td => td.Type == FanTableType.CPUSensor)
                    .Select(td => GetDescription(td, index, value))
                    .FirstOrDefault() ?? "-";
            }
            catch
            {
                _value2.Text = "-";
            }

            try
            {
                _value3.Text = tableData
                    .Where(td => td.Type == FanTableType.GPU)
                    .Select(td => GetDescription(td, index, value))
                    .FirstOrDefault() ?? "-";
            }
            catch
            {
                _value3.Text = "-";
            }
        }

        private static string GetDescription(FanTableData tableData, int index, int value)
        {
            var rpm = value < 0 ? 0 : tableData.FanSpeeds[value];
            return $"{tableData.Temps[index]}{Resource.Celsius} @ {rpm} {Resource.FanCurveControl_RPM}";
        }
    }
}