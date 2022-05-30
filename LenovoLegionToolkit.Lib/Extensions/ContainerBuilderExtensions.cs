using Autofac;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static void Register<T>(this ContainerBuilder cb, bool autoCreate = false) where T : notnull
        {
            var r = cb.RegisterType<T>().AsSelf().AsImplementedInterfaces().SingleInstance();

            if (autoCreate)
                r.AutoActivate();
        }
    }
}
