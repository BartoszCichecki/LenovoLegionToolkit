using System.Windows;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard.GodMode;

public partial class GodModeSliderControl
{
    private string _unit = string.Empty;

    public string Title
    {
        get => _cardControlHeader.Title;
        set => _cardControlHeader.Title = value;
    }

    public string Description
    {
        get => _cardControlHeader.Subtitle;
        set => _cardControlHeader.Subtitle = value;
    }

    public string Unit
    {
        get => _unit;
        set
        {
            _unit = value;
            _label.ContentStringFormat = $"{0} {_unit}";
        }
    }

    public double Minimum
    {
        get => _slider.Minimum;
        set => _slider.Minimum = value;
    }

    public double Maximum
    {
        get => _slider.Maximum;
        set => _slider.Maximum = value;
    }

    public double TickFrequency
    {
        get => _slider.TickFrequency;
        set => _slider.TickFrequency = value;
    }

    public double Value
    {
        get => _slider.Value;
        set => _slider.Value = value;
    }

    public bool IsSliderEnabled
    {
        get => _slider.IsEnabled;
        set => _slider.IsEnabled = value;
    }

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => _slider.ValueChanged += value;
        remove => _slider.ValueChanged -= value;
    }

    public GodModeSliderControl() => InitializeComponent();
}