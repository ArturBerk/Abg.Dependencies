using System;

namespace Abg.Dependencies
{
    internal class SingleRegistration<T> : IRegistration
    {
        private readonly Func<IContainer, T> factory;
        private T instance;
        private readonly Action<ResolvedInstance<T>> onActivate;
        private readonly bool autoActivate;

        public SingleRegistration(Func<IContainer, T> factory, Action<ResolvedInstance<T>> onActivate, 
            bool autoActivate)
        {
            this.factory = factory;
            this.onActivate = onActivate;
            this.autoActivate = autoActivate;
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