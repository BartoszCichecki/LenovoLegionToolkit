using Autofac;
using Autofac.Builder;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class ContainerBuilderExtensions
{
    public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Register<T>(this ContainerBuilder cb, bool selfOnly = false) where T : notnull
    {
        var registration = cb.RegisterType<T>().AsSelf();
        if (!selfOnly)
            registration = registration.AsImplementedInterfaces();
        return registration.SingleInstance();
    }
}
