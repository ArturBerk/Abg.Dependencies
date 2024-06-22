using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{

    internal abstract class RegistrationBuilderBase<T> : IRegistrationBuilder<T>
    {
        protected TypeCollection RegisterAs;
        protected List<IFactoryBuilder> FactoryBuilders;
        protected bool IsTransient = true;
        protected bool IsAutoActivate = true;
        protected Action<ResolvedInstance<T>> OnActivateAction;

        public Type Type { get; }

        public abstract IEnumerable<RegistrationInstance> Build();

        protected IEnumerable<RegistrationInstance> BuildFrom(IRegistration registration)
        {
            foreach (Type type in RegisterAs)
            {
                yield return new RegistrationInstance(type, registration);
            }

            if (FactoryBuilders != null)
            {
                foreach (var factoryBuilder in FactoryBuilders)
                {
                    yield return factoryBuilder.Build(registration);
                }
            }
        }

        protected RegistrationBuilderBase(Type type)
        {
            Type = type;
            RegisterAs = new TypeCollection(type);
        }
        
        IRegistrationBuilder IRegistrationBuilder.WithFactory<T1>()
        {
            return WithFactory<T1>();
        }

        public IRegistrationBuilder<T> WithFactory<T1>()
        {
            if (FactoryBuilders == null) FactoryBuilders = new List<IFactoryBuilder>();
            FactoryBuilders.Add(new FactoryBuilder<T1>());
            return this;
        }
        
        IRegistrationBuilder IRegistrationBuilder.WithFactory()
        {
            return WithFactory();
        }

        public IRegistrationBuilder<T> WithFactory()
        {
            return WithFactory<T>();
        }
        
        IRegistrationBuilder IRegistrationBuilder.As<T1>()
        {
            return As<T1>();
        }

        public IRegistrationBuilder<T> As<T1>()
        {
            RegisterAs.Add(typeof(T1));
            return this;
        }
        
        IRegistrationBuilder IRegistrationBuilder.As(Type type)
        {
            return As(type);
        }

        public IRegistrationBuilder<T> As(Type type)
        {
            RegisterAs.Add(type);
            return this;
        }

        IRegistrationBuilder IRegistrationBuilder.Transient()
        {
            return Transient();
        }

        public IRegistrationBuilder<T> Transient()
        {
            IsTransient = true;
            return this;
        }

        IRegistrationBuilder IRegistrationBuilder.Single()
        {
            return Single();
        }

        public IRegistrationBuilder<T> Single()
        {
            IsTransient = false;
            return this;
        }

        IRegistrationBuilder IRegistrationBuilder.AutoActivate()
        {
            return AutoActivate();
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