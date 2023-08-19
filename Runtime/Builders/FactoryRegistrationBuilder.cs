using System;

namespace Abg.Dependencies
{
    public class FactoryRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        private readonly Func<IContainer,T> factory;

        public FactoryRegistrationBuilder(Func<IContainer,T> factory) : base(typeof(T))
        {
            this.factory = factory;
        }

        public override IRegistration Build()
        {
            return IsTransient 
                ? new TransientFactoryRegistration<T>(factory, RegisterAs, OnActivateAction)
                : new SingleFactoryRegistration<T>(factory, RegisterAs, OnActivateAction,IsAutoActivate);
        }
    }
}