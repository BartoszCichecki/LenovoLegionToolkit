using Autofac;
using Autofac.Builder;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Register<T>(this ContainerBuilder cb) where T : notnull
        {
            return cb.RegisterType<T>().AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}
