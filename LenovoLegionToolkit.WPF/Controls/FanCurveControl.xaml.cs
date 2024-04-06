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
    private readonly List<Slider> _sliders = [];
    private readonly InfoTooltip _customToolTip = new();

    private FanTableData[]? _tableData;
    private FanTable? _minimumFanTable;

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

    public void SetFanTableInfo(FanTableInfo fanTableInfo, FanTable minimumFanTable)
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
        _minimumFanTable = minimumFanTable;

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

        if (sender is not Slider currentSlider)
            return;

        if (currentSlider is { IsKeyboardFocusWithin: false, IsMouseCaptureWithin: false })
            return;

        if (_minimumFanTable.HasValue)
        {
            var index = (int)currentSlider.Tag;
            var minimum = _minimumFanTable.Value.GetTable();

            if (currentSlider.Value < minimum[index])
            {
                currentSlider.Value = minimum[index];
                return;
            }
        }

        VerifyValues(currentSlider);
        DrawGraph();
    }

    private static CustomPopupPlacement[] ToolTipCustomPopupPlacementCallback(Size size, Size targetSize, Point _)
    {
        return
        [
            new(new((targetSize.Width - size.Width) * 0.5, -targetSize.Height -size.Height + 8), PopupPrimaryAxis.Vertical)
        ];
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
        var color = Application.Current.Resources["ControlFillColorDefaultBrush"] as SolidColorBrush;

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
            Data = new PathGeometry { Figures = [pathFigure] },
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
                new() { Height = GridLength.Auto},
                new() { Height = GridLength.Auto}
            }
        };

        private readonly TextBlock _cpuDescription = new() { Text = Resource.FanCurveControl_CPU, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };
        private readonly TextBlock _cpuSensorDescription = new() { Text = Resource.FanCurveControl_CPUSensor, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };
        private readonly TextBlock _gpuDescription = new() { Text = Resource.FanCurveControl_GPU, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };
        private readonly TextBlock _gpu2Description = new() { Text = Resource.FanCurveControl_GPU2, FontWeight = FontWeights.Medium, Margin = new(0, 0, 8, 0) };

        private readonly TextBlock _cpuValue = new();
        private readonly TextBlock _cpuSensorValue = new();
        private readonly TextBlock _gpuValue = new();
        private readonly TextBlock _gpu2Value = new();

        public InfoTooltip()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SetResourceReference(StyleProperty, typeof(ToolTip));

            Grid.SetColumn(_cpuDescription, 0);
            Grid.SetColumn(_cpuSensorDescription, 0);
            Grid.SetColumn(_gpuDescription, 0);
            Grid.SetColumn(_gpu2Description, 0);
            Grid.SetColumn(_cpuValue, 1);
            Grid.SetColumn(_cpuSensorValue, 1);
            Grid.SetColumn(_gpuValue, 1);
            Grid.SetColumn(_gpu2Value, 1);

            Grid.SetRow(_cpuDescription, 0);
            Grid.SetRow(_cpuSensorDescription, 1);
            Grid.SetRow(_gpuDescription, 2);
            Grid.SetRow(_gpu2Description, 3);
            Grid.SetRow(_cpuValue, 0);
            Grid.SetRow(_cpuSensorValue, 1);
            Grid.SetRow(_gpuValue, 2);
            Grid.SetRow(_gpu2Value, 3);

            _grid.Children.Add(_cpuDescription);
            _grid.Children.Add(_cpuSensorDescription);
            _grid.Children.Add(_gpuDescription);
            _grid.Children.Add(_gpu2Description);
            _grid.Children.Add(_cpuValue);
            _grid.Children.Add(_cpuSensorValue);
            _grid.Children.Add(_gpuValue);
            _grid.Children.Add(_gpu2Value);

            Content = _grid;
        }

        public void Update(FanTableData[] tableData, int index, int value)
        {
            Update(tableData, index, value, FanTableType.CPU, _cpuDescription, _cpuValue);
            Update(tableData, index, value, FanTableType.CPUSensor, _cpuSensorDescription, _cpuSensorValue);
            Update(tableData, index, value, FanTableType.GPU, _gpuDescription, _gpuValue);
            Update(tableData, index, value, FanTableType.GPU2, _gpu2Description, _gpu2Value);
        }

        private static void Update(FanTableData[] tableData, int index, int value, FanTableType type, TextBlock descriptionTextBlock, TextBlock valueTextBlock)
        {
            var text = tableData
                .Where(td => td.Type == type)
                .Select(td => GetDescription(td, index, value))
                .FirstOrDefault();

            var visibility = text is null ? Visibility.Collapsed : Visibility.Visible;

            valueTextBlock.Text = text ?? "-";
            valueTextBlock.Visibility = visibility;
            descriptionTextBlock.Visibility = visibility;
        }

        private static string GetDescription(FanTableData tableData, int index, int value)
        {
            try
            {
                var temp = tableData.Temps[index];

                if (temp >= 127)
                    return "-";

                var rpm = value < 0 ? 0 : tableData.FanSpeeds[value];
                return $"{temp}{Resource.Celsius} @ {rpm} {Resource.RPM}";
            }
            catch
            {
                return "-";
            }
        }
    }
}
