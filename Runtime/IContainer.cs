using System;
using System.Collections;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public interface IContainer : IAsyncDisposable
    {
        bool TryResolve<T>(out T service);
        bool TryResolve(Type type, out object service);

        T Resolve<T>();
        object Resolve(Type type);

        IEnumerable<T> ResolveAll<T>();
        IEnumerable ResolveAll(Type type);
    }
}