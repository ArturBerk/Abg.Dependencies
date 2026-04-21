using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abg.Dependencies
{
    public sealed class InstanceContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, object> store = new Dictionary<Type, object>();
        private readonly List<object> allInstances = new List<object>();
        private bool disposed;

        public Binder<T> Add<T>(T instance) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            allInstances.Add(instance);
            Register<T>(instance);
            return new Binder<T>(this, instance);
        }

        private void Register<T>(T instance) where T : class
        {
            if (store.TryGetValue(typeof(T), out var existing))
                ((List<T>)existing).Add(instance);
            else
                store[typeof(T)] = new List<T> { instance };
        }

        public T Resolve<T>() where T : class
        {
            if (TryResolve<T>(out var service))
                return service!;
            throw new InvalidOperationException($"Resolver for {typeof(T)} not registered.");
        }

        public bool TryResolve<T>(out T? service) where T : class
        {
            if (store.TryGetValue(typeof(T), out var existing))
            {
                var list = (List<T>)existing;
                if (list.Count > 0)
                {
                    service = list[0];
                    return true;
                }
            }
            service = null;
            return false;
        }

        public IReadOnlyList<T> GetAll<T>() where T : class
        {
            if (store.TryGetValue(typeof(T), out var existing))
                return (List<T>)existing;
            return Array.Empty<T>();
        }

        public void Build<T>(T value) where T : class
        {
            throw new InvalidOperationException($"Builder for {typeof(T)} not registered.");
        }

        public bool TryBuild<T>(T service) where T : class => false;

        public async ValueTask DisposeAsync()
        {
            if (disposed) return;
            disposed = true;

            for (var i = 0; i < allInstances.Count; i++)
            {
                var instance = allInstances[i];
                if (instance is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
                else if (instance is IDisposable disp)
                    disp.Dispose();
            }

            allInstances.Clear();
            store.Clear();
        }

        public readonly struct Binder<T> where T : class
        {
            private readonly InstanceContainer container;
            private readonly T instance;

            internal Binder(InstanceContainer container, T instance)
            {
                this.container = container;
                this.instance = instance;
            }

            public Binder<T> As<TService>() where TService : class
            {
                if (!(instance is TService service))
                    throw new InvalidOperationException(
                        $"Instance of type {instance.GetType()} is not assignable to {typeof(TService)}.");
                container.Register<TService>(service);
                return this;
            }
        }
    }
}