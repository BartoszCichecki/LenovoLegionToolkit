using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace LenovoLegionToolkit.WPF.Behaviors;

public class ProgressBarAnimateBehavior : Behavior<ProgressBar>
{
    private bool _isAnimating;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.ValueChanged += ProgressBar_ValueChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.ValueChanged -= ProgressBar_ValueChanged;
    }

    private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (sender is not ProgressBar progressBar)
            return;

        if (_isAnimating)
            return;

        _isAnimating = true;

        var doubleAnimation = new DoubleAnimation(e.OldValue,
            e.NewValue,
            new Duration(TimeSpan.FromMilliseconds(250)),
            FillBehavior.Stop);
        doubleAnimation.Completed += Completed;

        progressBar.BeginAnimation(RangeBase.ValueProperty, doubleAnimation);

        e.Handled = true;
    }

    private void Completed(object? sender, EventArgs e) => _isAnimating = false;
}
