using System;
using Autofac;

namespace LenovoLegionToolkit.Lib;

public class IoCContainer
{
    private static readonly object _lock = new();

    private static IContainer? _container;

    public static void Initialize(params Module[] modules)
    {
        lock (_lock)
        {
            if (_container is not null)
                throw new InvalidOperationException("IoCContainer already initialized");

            var cb = new ContainerBuilder();

            foreach (var module in modules)
                cb.RegisterModule(module);

            _container = cb.Build();
        }
    }

    public static T Resolve<T>() where T : notnull
    {
        lock (_lock)
        {
            if (_container is null)
                throw new InvalidOperationException($"IoCContainer must be initialized first. [type={nameof(T)}]");
            return _container.Resolve<T>();
        }
    }

    public static T? TryResolve<T>() where T : class
    {
        lock (_lock)
        {
            if (_container is null)
                throw new InvalidOperationException($"IoCContainer must be initialized first [type={nameof(T)}]");
            _ = _container.TryResolve(out T? value);
            return value;
        }
    }

}