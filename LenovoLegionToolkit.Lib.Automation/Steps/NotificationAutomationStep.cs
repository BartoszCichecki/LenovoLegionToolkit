using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LenovoLegionToolkit.Lib.Automation.Steps;

public class NotificationAutomationStep : IAutomationStep
{
    public string? Text { get; }

    [JsonConstructor]
    public NotificationAutomationStep(string? text)
    {
        Text = text;
    }

    public Task<bool> IsSupportedAsync() => Task.FromResult(true);

    public Task RunAsync(AutomationEnvironment environment)
    {
        if (!string.IsNullOrWhiteSpace(Text))
            MessagingCenter.Publish(new Notification(NotificationType.AutomationNotification, Text));
        return Task.CompletedTask;
    }

    IAutomationStep IAutomationStep.DeepCopy() => new NotificationAutomationStep(Text);
}
