using System;
using System.Collections;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public interface IContainer : IAsyncDisposable, IAsyncInitializable
    {
        bool TryResolve<T>(out T service);
        bool TryResolve(Type type, out object service);
        bool TryResolve<T>(out T service, bool includeParent = true);
        bool TryResolve(Type type, out object service, bool includeParent = true);

        T Resolve<T>();
        object Resolve(Type type);
        T Resolve<T>(bool includeParent);
        object Resolve(Type type, bool includeParent);

        IEnumerable<T> ResolveAll<T>(bool includeParent = true);
        IEnumerable ResolveAll(Type type, bool includeParent = true);
        IEnumerable<T> ResolveAll<T>();
        IEnumerable ResolveAll(Type type);
    }
}