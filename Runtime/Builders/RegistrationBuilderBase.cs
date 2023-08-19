using System;

namespace Abg.Dependencies
{
    public abstract class RegistrationBuilderBase<T> : IRegistrationBuilder<T>
    {
        protected TypeCollection RegisterAs;
        protected bool IsTransient = true;
        protected bool IsAutoActivate = true;
        protected Action<ResolvedInstance<T>> OnActivateAction;
        
        public abstract IRegistration Build();

        public Type Type { get; }

        protected RegistrationBuilderBase(Type type)
        {
            Type = type;
            RegisterAs = new TypeCollection(type);
        }

        public IRegistrationBuilder<T> As<T1>()
        {
            RegisterAs.Add(typeof(T1));
            return this;
        }

        public IRegistrationBuilder<T> As(Type type)
        {
            RegisterAs.Add(type);
            return this;
        }

        public IRegistrationBuilder<T> Transient()
        {
            IsTransient = true;
            return this;
        }

        public IRegistrationBuilder<T> Single()
        {
            IsTransient = false;
            return this;
        }

        public IRegistrationBuilder<T> AutoActivate()
        {
            IsAutoActivate = true;
            return this;
        }

        public IRegistrationBuilder<T> OnActivated(Action<ResolvedInstance<T>> onActivate)
        {
            OnActivateAction = onActivate;
            return this;
        }
    }
}