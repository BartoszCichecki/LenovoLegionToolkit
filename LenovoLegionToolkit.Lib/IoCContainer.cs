using System;
using Autofac;

namespace LenovoLegionToolkit.Lib
{
    public class IoCContainer
    {
        private static IContainer? _container;

        public static void Initialize(params Module[] modules)
        {
            var cb = new ContainerBuilder();

            foreach (var module in modules)
                cb.RegisterModule(module);

            _container = cb.Build();
        }

        public static T Resolve<T>() where T : notnull
        {
            if (_container is null)
                throw new InvalidOperationException("IoCContainer must be initialized first");
            return _container.Resolve<T>();
        }
    }
}
