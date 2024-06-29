namespace LenovoLegionToolkit.Lib.Messaging.Messages;

public readonly struct FeatureStateMessage<T>(T state) : IMessage
{
    public T State { get; } = state;
}
