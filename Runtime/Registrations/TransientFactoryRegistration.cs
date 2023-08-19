using System;

namespace Abg.Dependencies
{
    public class TransientFactoryRegistration<T> : IRegistration<T>
    {
        private readonly Func<IContainer, T> factory;
        private readonly Action<ResolvedInstance<T>> onActivate;

        public TypeCollection RegisterAs { get; }

        public TransientFactoryRegistration(Func<IContainer, T> factory, TypeCollection registerAs, 
            Action<ResolvedInstance<T>> onActivate)
        {
            this.factory = factory;
            this.onActivate = onActivate;
            RegisterAs = registerAs;
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