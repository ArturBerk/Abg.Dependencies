using System;

namespace Abg.Dependencies
{
    public class SingleFactoryRegistration<T> : IRegistration<T>
    {
        private readonly Func<IContainer, T> factory;
        private T instance;
        private readonly Action<ResolvedInstance<T>> onActivate;
        private readonly bool autoActivate;

        public TypeCollection RegisterAs { get; }

        public SingleFactoryRegistration(Func<IContainer, T> factory, TypeCollection registerAs, 
            Action<ResolvedInstance<T>> onActivate, bool autoActivate)
        {
            this.factory = factory;
            this.onActivate = onActivate;
            this.autoActivate = autoActivate;
            RegisterAs = registerAs;
        }

        public T Resolve(IContainer container)
        {
            if (instance == null) instance = factory(container);
            onActivate?.Invoke(new ResolvedInstance<T>(container, instance));
            return instance;
        }

        public void Activate(IContainer container)
        {
            if (autoActivate)
                instance = Resolve(container);
        }

        object IRegistration.Resolve(IContainer container)
        {
            return Resolve(container);
        }
    }
}