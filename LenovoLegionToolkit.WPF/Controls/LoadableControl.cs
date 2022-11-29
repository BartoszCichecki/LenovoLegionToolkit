using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls;

public class LoadableControl : UserControl
{
    private readonly ContentPresenter _contentPresenter = new();

    private readonly ProgressRing _progressRing = new()
    {
        IsIndeterminate = true,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        Width = 48,
        Height = 48,
    };

    private bool _isLoading = true;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            UpdateLoadingState();
        }
    }

    public bool IsIndeterminate
    {
        get => _progressRing.IsIndeterminate;
        set => _progressRing.IsIndeterminate = value;
    }

    public double Progress
    {
        get => _progressRing.Progress;
        set => _progressRing.Progress = value;
    }

    public double IndicatorWidth
    {
        get => _progressRing.Width;
        set => _progressRing.Width = value;
    }

    public double IndicatorHeight
    {
        get => _progressRing.Height;
        set => _progressRing.Height = value;
    }

    public HorizontalAlignment IndicatorHorizontalAlignment
    {
        get => _progressRing.HorizontalAlignment;
        set => _progressRing.HorizontalAlignment = value;
    }

    public VerticalAlignment IndicatorVerticalAlignment
    {
        get => _progressRing.VerticalAlignment;
        set => _progressRing.VerticalAlignment = value;
    }

    public Visibility ContentVisibilityWhileLoading { get; set; } = Visibility.Hidden;

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        _contentPresenter.Content = Content;

        _progressRing.RenderTransformOrigin = new(0.5, 0.5);
        _progressRing.RenderTransform = new TransformGroup
        {
            Children =
            {
                new RotateTransform(-90),
                new ScaleTransform(-1, 1),
            }
        };

        var grid = new Grid();
        grid.Children.Add(_contentPresenter);
        grid.Children.Add(_progressRing);

        UpdateLoadingState();

        Content = grid;
    }

    private void UpdateLoadingState()
    {
        _contentPresenter.Visibility = IsLoading ? ContentVisibilityWhileLoading : Visibility.Visible;
        _progressRing.Visibility = IsLoading ? Visibility.Visible : Visibility.Hidden;
    }
}