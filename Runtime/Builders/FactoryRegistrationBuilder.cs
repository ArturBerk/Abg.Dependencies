using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    internal class FactoryRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        private readonly Func<IContainer,T> factory;

        public FactoryRegistrationBuilder(Func<IContainer,T> factory) : base(typeof(T))
        {
            this.factory = factory;
        }

        public override IEnumerable<RegistrationInstance> Build()
        {
            return BuildFrom(IsTransient 
                ? new TransientRegistration<T>(factory, OnActivateAction)
                : new SingleRegistration<T>(factory, OnActivateAction,IsAutoActivate));
        }
    }
}