using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abg.Dependencies
{
    public sealed class Container : IContainer
    {
        private readonly IContainer parent;
        private readonly Dictionary<Type, RegistrationList> registrations = new Dictionary<Type, RegistrationList>();

        public Container(IContainer parent, IReadOnlyCollection<RegistrationInstance> registrations)
        {
            this.parent = parent;
            this.registrations.Add(typeof(IContainer),
                new RegistrationList(
                    new InstanceRegistration<IContainer>(this, null)));
            foreach (RegistrationInstance registration in registrations)
            {
                    if (!this.registrations.TryGetValue(registration.Type, out var list))
                    {
                        list = new RegistrationList(registration.Registration);
                        this.registrations.Add(registration.Type, list);
                        continue;
                    }

                    list.Add(registration.Registration);
            }

            foreach (RegistrationInstance registration in registrations)
            {
                registration.Registration.Activate(this);
            }
        }

        public async Task InitializeAsync()
        {
            var asyncInitializables = ResolveAll<IAsyncInitializable>(false);
            var initializables = ResolveAll<IInitializable>(false);
            var asyncInitialization = Task.WhenAll(asyncInitializables.Select(i => i.InitializeAsync()));
            
            foreach (IInitializable initializable in initializables)
            {
                initializable.Initialize();
            }

            await asyncInitialization;
        }

        public async ValueTask DisposeAsync()
        {
            var asyncDisposables = ResolveAll<IAsyncDisposable>(false);
            var disposables = ResolveAll<IDisposable>(false);
            var asyncDisposing = Task.WhenAll(asyncDisposables.Select(i => i.DisposeAsync().AsTask()));
            
            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }

            await asyncDisposing;
        }

        public bool TryResolve<T>(out T service)
        {
            return TryResolve(out service, true);
        }

        public bool TryResolve<T>(out T service, bool includeParent)
        {
            if (!this.registrations.TryGetValue(typeof(T), out var registrations))
            {
                if (includeParent && parent != null)
                    return parent.TryResolve(out service);
                service = default;
                return false;
            }

            service = (T)registrations.Resolve(this);
            return true;
        }

        public bool TryResolve(Type type, out object service)
        {
            return TryResolve(type, out service, true);
        }
        
        public bool TryResolve(Type type, out object service, bool includeParent)
        {
            if (!this.registrations.TryGetValue(type, out var registrations))
            {
                if (includeParent && parent != null)
                    return parent.TryResolve(type, out service);
                service = null;
                return false;
            }

            service = registrations.Resolve(this);
            return true;
        }

        public T Resolve<T>()
        {
            return Resolve<T>(true);
        }
        
        public T Resolve<T>(bool includeParent)
        {
            if (!TryResolve<T>(out var result))
                throw new Exception($"Type {typeof(T)} not registered");

            return result;
        }

        public object Resolve(Type type)
        {
            return Resolve(type, true);
        }
        
        public object Resolve(Type type, bool includeParent)
        {
            if (!TryResolve(type, out var result))
                throw new Exception($"Type {type} not registered");

            return result;
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ResolveAll<T>(true);
        }

        public IEnumerable<T> ResolveAll<T>(bool includeParent)
        {
            if (!this.registrations.TryGetValue(typeof(T), out var registrations))
                yield break;

            foreach (IRegistration registration in registrations.ResolveAll())
            {
                yield return (T)registration.Resolve(this);
            }
            
            if (includeParent && parent != null)
            {
                foreach (var t in parent.ResolveAll<T>(true))
                {
                    yield return t;
                }
            }
        }

        public IEnumerable ResolveAll(Type type)
        {
            return ResolveAll(type, true);
        }

        public IEnumerable ResolveAll(Type type, bool includeParent)
        {
            if (!this.registrations.TryGetValue(type, out var registrations))
                yield break;

            foreach (IRegistration registration in registrations.ResolveAll())
            {
                yield return registration.Resolve(this);
            }

            if (includeParent && parent != null)
            {
                foreach (var t in parent.ResolveAll(type, true))
                {
                    yield return t;
                }
            }
        }

        private class RegistrationList
        {
            private readonly IRegistration registration;
            private List<IRegistration> registrations;

            public RegistrationList(IRegistration registration)
            {
                this.registration = registration;
            }

            public void Add(IRegistration registration)
            {
                if (registrations == null)
                {
                    registrations = new List<IRegistration> { this.registration };
                }

                registrations.Add(registration);
            }

            public object Resolve(IContainer container)
            {
                return registration.Resolve(container);
            }

            public IEnumerable ResolveAll()
            {
                if (registrations == null)
                {
                    yield return registration;
                    yield break;
                }

                foreach (IRegistration registration1 in registrations)
                {
                    yield return registration1;
                }
            }
        }
    }
}