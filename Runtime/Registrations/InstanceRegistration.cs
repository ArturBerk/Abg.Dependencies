using System;

namespace Abg.Dependencies
{
    public class InstanceRegistration<T> : IRegistration<T>
    {
        private readonly T instance;
        private readonly bool autoActivate;
        private Action<ResolvedInstance<T>> onActivate;

        public TypeCollection RegisterAs { get; }

        public InstanceRegistration(T instance, TypeCollection registerAs, Action<ResolvedInstance<T>> onActivate, bool autoActivate)
        {
            this.instance = instance;
            RegisterAs = registerAs;
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
            return instance;
        }
    }
}