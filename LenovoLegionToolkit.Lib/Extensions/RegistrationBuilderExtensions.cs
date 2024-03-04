using System;
using Autofac;
using Autofac.Builder;
using LenovoLegionToolkit.Lib.Listeners;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class RegistrationBuilderExtensions
{
    public static void AutoActivateListener<T>(this IRegistrationBuilder<IListener<T>, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration) where T : EventArgs
    {
        registration.OnActivating(e => e.Instance.StartAsync().AsValueTask()).AutoActivate();
    }
}
