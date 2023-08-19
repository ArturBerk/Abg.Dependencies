using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Abg.Dependencies
{
    public class Container : IContainer
    {
        private readonly IContainer parent;
        private readonly Dictionary<Type, RegistrationList> registrations = new Dictionary<Type, RegistrationList>();

        public Container(IContainer parent, IRegistration[] registrations)
        {
            this.parent = parent;
            this.registrations.Add(typeof(IContainer),
                new RegistrationList(
                    new InstanceRegistration<IContainer>(this, new TypeCollection(typeof(IContainer)), null, false)));
            foreach (IRegistration registration in registrations)
            {
                foreach (var registerAsType in registration.RegisterAs)
                {
                    if (!this.registrations.TryGetValue(registerAsType, out var list))
                    {
                        list = new RegistrationList(registration);
                        this.registrations.Add(registerAsType, list);
                        continue;
                    }

                    list.Add(registration);
                }
            }

            foreach (IRegistration registration in registrations)
            {
                registration.Activate(this);
            }
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in ResolveAll<IDisposable>())
            {
                disposable.Dispose();
            }
        }

        public bool TryResolve<T>(out T service)
        {
            if (!this.registrations.TryGetValue(typeof(T), out var registrations))
            {
                if (parent != null)
                    return parent.TryResolve(out service);
                service = default;
                return false;
            }

            service = (T)registrations.Resolve(this);
            return true;
        }

        public bool TryResolve(Type type, out object service)
        {
            if (!this.registrations.TryGetValue(type, out var registrations))
            {
                if (parent != null)
                    return parent.TryResolve(type, out service);
                service = null;
                return false;
            }

            service = registrations.Resolve(this);
            return true;
        }

        public T Resolve<T>()
        {
            if (!TryResolve<T>(out var result))
                throw new Exception($"Type {typeof(T)} not registered");

            return result;
        }

        public object Resolve(Type type)
        {
            if (!TryResolve(type, out var result))
                throw new Exception($"Type {type} not registered");

            return result;
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            if (!this.registrations.TryGetValue(typeof(T), out var registrations))
                yield break;

            foreach (IRegistration registration in registrations.ResolveAll())
            {
                yield return (T)registration.Resolve(this);
            }
        }

        public IEnumerable ResolveAll(Type type)
        {
            if (!this.registrations.TryGetValue(type, out var registrations))
                yield break;

            foreach (IRegistration registration in registrations.ResolveAll())
            {
                yield return registration.Resolve(this);
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
                    yield return registration;

                foreach (IRegistration registration1 in registrations)
                {
                    yield return registration1;
                }
            }
        }
    }
}