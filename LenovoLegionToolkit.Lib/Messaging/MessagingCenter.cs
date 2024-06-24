using System;
using LenovoLegionToolkit.Lib.Messaging.Messages;
using PubSub;

namespace LenovoLegionToolkit.Lib.Messaging;

public static class MessagingCenter
{
    public static void Publish<T>(T data) where T : IMessage => Hub.Default.Publish(data);

    public static void Subscribe<T>(object subscriber, Action<T> handler) where T : IMessage => Hub.Default.Subscribe(subscriber, handler);

    public static void Subscribe<T>(object subscriber, Action handler) where T : IMessage => Hub.Default.Subscribe<T>(subscriber, _ => handler());
}
