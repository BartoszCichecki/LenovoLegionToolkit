using System;
using PubSub;

namespace LenovoLegionToolkit.Lib;

public static class MessagingCenter
{
    public static void Publish<T>(T data) => Hub.Default.Publish(data);

    public static void Subscribe<T>(object subscriber, Action<T> handler) => Hub.Default.Subscribe(subscriber, handler);

    public static void Subscribe<T>(object subscriber, Action handler) => Hub.Default.Subscribe<T>(subscriber, _ => handler());
}