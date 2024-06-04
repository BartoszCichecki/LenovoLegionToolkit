using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Macro;

public class MacroRecordingWindow : UiWindow
{
    private readonly Grid _mainGrid = new()
    {
        RowDefinitions =
        {
            new() { Height = GridLength.Auto },
            new() { Height = GridLength.Auto },
        },
        ColumnDefinitions =
        {
            new() { Width = GridLength.Auto, },
            new() { Width = new(1, GridUnitType.Star) },
        },
        Margin = new(16, 16, 32, 16),
    };

    private readonly SymbolIcon _symbolIcon = new()
    {
        FontSize = 32,
        Margin = new(0, 0, 16, 0),
    };

    private readonly Label _titleLabel = new()
    {
        FontSize = 16,
        FontWeight = FontWeights.Medium,
        VerticalContentAlignment = VerticalAlignment.Center,
    };

    private readonly Label _subtitleLabel = new()
    {
        FontSize = 14,
        VerticalContentAlignment = VerticalAlignment.Center,
        Margin = new(0, 4, 0, 0),
    };

    public static MacroRecordingWindow CreatePreparing() => new(SymbolRegular.HourglassThreeQuarter24, Resource.MacroRecordingWindow_Preparing_Title, null);

    public static MacroRecordingWindow CreateRecording() => new(SymbolRegular.Record24, Resource.MacroRecordingWindow_Recording_Title, Resource.MacroRecordingWindow_Recording_Message);

    private MacroRecordingWindow(SymbolRegular symbol, string title, string? subtitle)
    {
        InitializeStyle();
        InitializeContent(symbol, title, subtitle);

        _mainGrid.Measure(new Size(double.PositiveInfinity, 80));

        Width = MaxWidth = MinWidth = Math.Max(_mainGrid.DesiredSize.Width, 300);
        Height = MaxHeight = MinHeight = _mainGrid.DesiredSize.Height;
    }

    private void InitializeStyle()
    {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Focusable = false;
        Topmost = true;
        ExtendsContentIntoTitleBar = true;
        ShowInTaskbar = false;
        ShowActivated = false;

        _mainGrid.FlowDirection = LocalizationHelper.Direction;
        _titleLabel.Foreground = (SolidColorBrush)FindResource("TextFillColorPrimaryBrush");
    }

    private void InitializeContent(SymbolRegular symbol, string title, string? subtitle)
    {
        _symbolIcon.Symbol = symbol;
        _titleLabel.Content = title;
        _subtitleLabel.Content = subtitle;


        Grid.SetRow(_symbolIcon, 0);
        Grid.SetRow(_titleLabel, 0);
        Grid.SetRow(_subtitleLabel, 1);

        Grid.SetColumn(_symbolIcon, 0);
        Grid.SetColumn(_titleLabel, 1);
        Grid.SetColumn(_subtitleLabel, 1);

        Grid.SetRowSpan(_symbolIcon, 2);

        if (subtitle is null)
            Grid.SetRowSpan(_titleLabel, 2);

        _mainGrid.Children.Add(_symbolIcon);
        _mainGrid.Children.Add(_titleLabel);
        _mainGrid.Children.Add(_subtitleLabel);

        Content = _mainGrid;
    }
}
