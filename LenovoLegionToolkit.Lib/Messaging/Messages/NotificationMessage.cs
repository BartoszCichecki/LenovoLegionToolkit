namespace LenovoLegionToolkit.Lib.Messaging.Messages;

public readonly struct NotificationMessage(NotificationType type, params object[] args) : IMessage
{
    public NotificationType Type { get; } = type;

    public object[] Args { get; } = args;

    public override string ToString() => $@"{nameof(Type)}: {Type}, {nameof(Args)}: [{string.Join(", ", Args)}]";
}
