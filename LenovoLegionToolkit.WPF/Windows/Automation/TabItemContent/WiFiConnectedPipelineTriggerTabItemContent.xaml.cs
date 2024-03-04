using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.System;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class WiFiConnectedPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<IWiFiConnectedPipelineTrigger>
{
    private readonly IWiFiConnectedPipelineTrigger _trigger;

    public WiFiConnectedPipelineTriggerTabItemContent(IWiFiConnectedPipelineTrigger trigger)
    {
        _trigger = trigger;

        InitializeComponent();

        trigger.Ssids
            .DefaultIfEmpty(string.Empty)
            .Select(CreateControl)
            .ForEach(c => _ssidStackPanel.Children.Add(c));
    }

    public IWiFiConnectedPipelineTrigger GetTrigger()
    {
        var ssids = _ssidStackPanel.Children
            .OfType<ItemControl>()
            .Select(c => c.Text)
            .Distinct()
            .Where(s => !string.IsNullOrEmpty(s))
            .ToArray();
        return _trigger.DeepCopy(ssids);
    }

    private void AddNetworkNameButton_OnClick(object sender, RoutedEventArgs e)
    {
        _ssidStackPanel.Children.Add(CreateControl(string.Empty));
    }

    private void CopyCurrentNetworkNameButton_OnClick(object sender, RoutedEventArgs e)
    {
        var ssid = WiFi.GetConnectedNetworkSsid();
        if (ssid is null)
            return;

        var last = _ssidStackPanel.Children
            .OfType<ItemControl>()
            .LastOrDefault();

        if (last is not null && string.IsNullOrEmpty(last.Text))
            last.Text = ssid;
        else
            _ssidStackPanel.Children.Add(CreateControl(ssid));
    }

    private UserControl CreateControl(string ssid)
    {
        var control = new ItemControl { Text = ssid };
        control.Delete += (s, _) =>
        {
            if (s is not UIElement element)
                return;

            _ssidStackPanel.Children.Remove(element);

            if (_ssidStackPanel.Children.OfType<ItemControl>().Any())
                return;

            _ssidStackPanel.Children.Add(CreateControl(string.Empty));
        };
        return control;
    }

    private class ItemControl : UserControl
    {
        private readonly TextBox _textBox = new();

        private readonly Button _removeButton = new()
        {
            Margin = new(8, 0, 0, 0),
            Icon = SymbolRegular.Delete24
        };

        private readonly Grid _grid = new()
        {
            Margin = new(0, 0, 0, 8),
            ColumnDefinitions =
            {
                new() { Width = new GridLength(1, GridUnitType.Star) },
                new() { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };

        public string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }

        public event EventHandler? Delete;

        public ItemControl()
        {
            _removeButton.SetBinding(HeightProperty, new Binding(nameof(_textBox.ActualHeight)) { Source = _textBox });
            _removeButton.Click += (_, _) => Delete?.Invoke(this, EventArgs.Empty);

            Grid.SetColumn(_textBox, 0);
            Grid.SetColumn(_removeButton, 1);

            _grid.Children.Add(_textBox);
            _grid.Children.Add(_removeButton);

            Content = _grid;
        }
    }
}
