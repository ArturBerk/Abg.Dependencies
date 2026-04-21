using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public interface IDependencyContainer : IAsyncDisposable
    {
        T Resolve<T>() where T : class;
        void Build<T>(T value) where T : class;
        IReadOnlyList<T> GetAll<T>() where T : class;
        bool TryResolve<T>(out T? service) where T : class;
        bool TryBuild<T>(T service) where T : class;
    }
}