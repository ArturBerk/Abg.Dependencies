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
        private readonly Action<ResolvedInstance<T>> onActivate;

        public InstanceRegistration(T instance, Action<ResolvedInstance<T>> onActivate)
        {
            this.instance = instance;
            this.onActivate = onActivate;
        }

        public void Activate(IContainer container)
        {
            onActivate?.Invoke(new ResolvedInstance<T>(container, instance));
        }

        object IRegistration.Resolve(IContainer container)
        {
            return instance;
        }
    }
}