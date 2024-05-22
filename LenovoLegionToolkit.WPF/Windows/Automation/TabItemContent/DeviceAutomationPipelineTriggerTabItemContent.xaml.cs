using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.System;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class DeviceAutomationPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<INativeWindowsMessagePipelineTrigger>
{
    private readonly NativeWindowsMessageListener _listener = IoCContainer.Resolve<NativeWindowsMessageListener>();

    private readonly IDeviceAutomationPipelineTrigger _trigger;

    private readonly HashSet<string> _instanceIds;

    private readonly List<Device> _devices = [];

    private bool _onlyRemovable = true;

    private CancellationTokenSource? _filterDebounceCancellationTokenSource;

    public DeviceAutomationPipelineTriggerTabItemContent(IDeviceAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _instanceIds = [.. trigger.InstanceIds];

        InitializeComponent();
    }

    private async void DeviceAutomationPipelineTriggerTabItemContent_Initialized(object? sender, EventArgs e)
    {
        _onlyRemovableButton.Appearance = _onlyRemovable ? ControlAppearance.Primary : ControlAppearance.Secondary;

        await LoadAsync();
    }

    private async void NativeWindowsMessageListener_Changed(object? sender, NativeWindowsMessageListener.ChangedEventArgs e)
    {
        if (e.Message is not NativeWindowsMessage.DeviceConnected and not NativeWindowsMessage.DeviceDisconnected)
            return;

        await LoadAsync();
    }

    private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (_filterDebounceCancellationTokenSource is not null)
                await _filterDebounceCancellationTokenSource.CancelAsync();

            _filterDebounceCancellationTokenSource = new();

            await Task.Delay(500, _filterDebounceCancellationTokenSource.Token);

            _content.Children.Clear();
            _scrollViewer.ScrollToHome();

            Reload();
        }
        catch (OperationCanceledException) { }
    }

    private void OnlyRemovableButton_Click(object sender, RoutedEventArgs e)
    {
        _onlyRemovable = !_onlyRemovable;
        _onlyRemovableButton.Appearance = _onlyRemovable ? ControlAppearance.Primary : ControlAppearance.Secondary;
        Reload();
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        _instanceIds.Clear();

        _content.Children
            .OfType<ListItem>()
            .Select(li => li.Device)
            .Select(d => d.DeviceInstanceId)
            .ForEach(i => _instanceIds.Add(i));

        Reload();
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        if (_instanceIds.IsEmpty())
            return;

        _instanceIds.Clear();

        Reload();
    }

    private async Task LoadAsync()
    {
        _listener.Changed -= NativeWindowsMessageListener_Changed;

        _devices.Clear();
        _devices.AddRange(await Task.Run(Devices.GetAll));

        _listener.Changed += NativeWindowsMessageListener_Changed;

        Reload();
    }

    private void Reload()
    {
        _content.Children.Clear();

        if (_devices.Count == 0)
            return;

        var devices = SortAndFilter(_devices);

        foreach (var device in devices)
        {
            var listItem = new ListItem(device)
            {
                IsChecked = _instanceIds.Contains(device.DeviceInstanceId)
            };
            listItem.Checked += (_, _) => _instanceIds.Add(device.DeviceInstanceId);
            listItem.Unchecked += (_, _) => _instanceIds.Remove(device.DeviceInstanceId);
            _content.Children.Add(listItem);
        }
    }

    private List<Device> SortAndFilter(List<Device> devices)
    {
        var result = devices.AsEnumerable();
        if (_onlyRemovable)
            result = result.Where(d => d.IsRemovable);
        result = result.OrderBy(d => d.Name);

        if (!string.IsNullOrWhiteSpace(_filterTextBox.Text))
            result = result.Where(p => p.Index.Contains(_filterTextBox.Text, StringComparison.InvariantCultureIgnoreCase));

        return result.ToList();
    }

    public INativeWindowsMessagePipelineTrigger GetTrigger() => _trigger.DeepCopy([.. _instanceIds]);

    private class ListItem : UserControl
    {
        private readonly CardControl _cardControl = new()
        {
            Margin = new(0, 0, 0, 8)
        };

        private readonly StackPanel _stackPanel = new()
        {
            Orientation = Orientation.Vertical
        };

        private readonly TextBlock _notConnected = new()
        {
            Margin = new(0, 0, 0, 2),
            TextTrimming = TextTrimming.CharacterEllipsis,
            FontSize = 12,
            Text = "Not connected"
        };

        private readonly TextBlock _name = new()
        {
            Margin = new(0, 0, 0, 4),
            FontWeight = FontWeights.Medium
        };

        private readonly TextBlock _description = new()
        {
            Margin = new(0, 0, 0, 4)
        };

        private readonly TextBlock _busReportedDeviceDescription = new()
        {
            Margin = new(0, 0, 0, 4)
        };

        private readonly TextBlock _deviceInstanceId = new()
        {
            Margin = new(0, 4, 0, 0),
            TextTrimming = TextTrimming.CharacterEllipsis,
            FontSize = 12
        };

        private readonly CheckBox _checkBox = new();

        public Device Device { get; }

        public bool? IsChecked
        {
            get => _checkBox.IsChecked;
            set => _checkBox.IsChecked = value;
        }

        public event RoutedEventHandler Checked
        {
            add => _checkBox.Checked += value;
            remove => _checkBox.Checked -= value;
        }

        public event RoutedEventHandler Unchecked
        {
            add => _checkBox.Unchecked += value;
            remove => _checkBox.Unchecked -= value;
        }

        public ListItem(Device device)
        {
            Device = device;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            if (Device.IsDisconnected)
            {
                _notConnected.SetResourceReference(ForegroundProperty, "SystemFillColorCautionBrush");
                _stackPanel.Children.Add(_notConnected);
            }

            _name.Text = Device.Name;
            _stackPanel.Children.Add(_name);

            if (Device.Description.Length > 0 && Device.Description != Device.Name)
            {
                _description.Text = Device.Description;
                _stackPanel.Children.Add(_description);
            }

            if (Device.BusReportedDeviceDescription.Length > 0 &&
                Device.BusReportedDeviceDescription != Device.Name &&
                Device.BusReportedDeviceDescription != Device.Description)
            {
                _busReportedDeviceDescription.Text = Device.BusReportedDeviceDescription;
                _stackPanel.Children.Add(_busReportedDeviceDescription);
            }

            _deviceInstanceId.Text = Device.DeviceInstanceId;
            _deviceInstanceId.SetResourceReference(ForegroundProperty, "TextFillColorSecondaryBrush");
            _stackPanel.Children.Add(_deviceInstanceId);

            _cardControl.Header = _stackPanel;
            _cardControl.Content = _checkBox;

            _cardControl.Click += (_, _) => _checkBox.IsChecked = !(_checkBox.IsChecked ?? false);

            Content = _cardControl;
        }
    }
}
