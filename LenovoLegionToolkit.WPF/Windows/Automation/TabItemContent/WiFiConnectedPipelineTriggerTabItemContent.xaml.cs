using System.Windows;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.System;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class WiFiConnectedPipelineTriggerTabItemContent : IAutomationPipelineTriggerTabItemContent<IWiFiConnectedPipelineTrigger>
{
    private readonly IWiFiConnectedPipelineTrigger _trigger;

    public WiFiConnectedPipelineTriggerTabItemContent(IWiFiConnectedPipelineTrigger trigger)
    {
        _trigger = trigger;

        InitializeComponent();

        _ssidTextBox.Text = trigger.Ssid ?? string.Empty;
    }

    public IWiFiConnectedPipelineTrigger GetTrigger()
    {
        var state = _ssidTextBox.Text;
        if (string.IsNullOrWhiteSpace(state))
            state = null;
        return _trigger.DeepCopy(state);
    }

    private void CopyCurrentNetworkNameButton_OnClick(object sender, RoutedEventArgs e)
    {
        _ssidTextBox.Text = WiFi.GetConnectedNetworkSSID() ?? string.Empty;
    }
}
