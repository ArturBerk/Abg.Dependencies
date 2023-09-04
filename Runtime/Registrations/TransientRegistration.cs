using System;

namespace Abg.Dependencies
{
    internal class TransientRegistration<T> : IRegistration
    {
        private readonly Func<IContainer, T> factory;
        private readonly Action<ResolvedInstance<T>> onActivate;

        public TransientRegistration(Func<IContainer, T> factory, Action<ResolvedInstance<T>> onActivate)
        {
            this.factory = factory;
            this.onActivate = onActivate;
        }

        public T Resolve(IContainer container)
        {
            var result = factory(container);
            onActivate?.Invoke(new ResolvedInstance<T>(container, result));
            return factory(container);
        }

        public void Activate(IContainer container)
        {
        }

        object IRegistration.Resolve(IContainer container)
        {
            return Resolve(container);
        }
    }
}