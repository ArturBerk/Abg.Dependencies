using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abg.Dependencies
{
    public abstract class ContainerBase : IDependencyContainer
    {
        public abstract T Resolve<T>() where T : class;
        public abstract void Build<T>(T value) where T : class;
        public abstract IReadOnlyList<T> GetAll<T>() where T : class;
        public abstract bool TryResolve<T>(out T? service) where T : class;
        public abstract bool TryBuild<T>(T service) where T : class;

        public abstract ValueTask DisposeAsync();

        public Binder<TImpl> Bind<TImpl>() where TImpl : class => default;
        public void Install<T>(IInstaller installer) where T : class { }
        public void Container<T>(T container) where T : IDependencyContainer { }

        public struct Binder<TImpl> where TImpl : class
        {
            public Binder<TImpl> As<TService>() where TService : class => default;
            public BinderOptions<TImpl> AsSingle() => default;
            public BinderOptions<TImpl> AsTransient() => default;
            public BinderOptions<TImpl> AsBuilder() => default;
            public BinderOptions<TImpl> ToField(TImpl field) => default;
            public BinderOptions<TImpl> ToFactory(Func<TImpl> field) => default;
        }

        public struct BinderOptions<TImpl> where TImpl : class
        {
            public void WithFactory() { }
        }
    }
}
