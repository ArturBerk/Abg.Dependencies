using System;

namespace Abg.Dependencies
{
    internal class FactoryRegistration<T> : IRegistration
    {
        private readonly IRegistration instanceRegistration;

        public FactoryRegistration(IRegistration instanceRegistration)
        {
            this.instanceRegistration = instanceRegistration;
        }

        public object Resolve(IContainer container)
        {
            return (Func<T>)(() => (T)instanceRegistration.Resolve(container));
        }

        public void Activate(IContainer container)
        {
        }
    }
    
    internal class InstanceRegistration<T> : IRegistration
    {
        private readonly T instance;
        private readonly bool autoActivate;
        private readonly Action<ResolvedInstance<T>> onActivate;

        public InstanceRegistration(T instance, Action<ResolvedInstance<T>> onActivate, bool autoActivate)
        {
            this.instance = instance;
            this.autoActivate = autoActivate;
            this.onActivate = onActivate;
        }

        public T Resolve(IContainer container)
        {
            onActivate?.Invoke(new ResolvedInstance<T>(container, instance));
            return instance;
        }

        public void Activate(IContainer container)
        {
            if (autoActivate)
                Resolve(container);
        }

        object IRegistration.Resolve(IContainer container)
        {
            onActivate?.Invoke(new ResolvedInstance<T>(container, instance));
            return instance;
        }
    }
}