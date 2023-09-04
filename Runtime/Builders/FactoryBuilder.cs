using System;

namespace Abg.Dependencies
{
    internal interface IFactoryBuilder
    {
        RegistrationInstance Build(IRegistration registration);
    }
    
    internal class FactoryBuilder<T> : IFactoryBuilder
    {
        public RegistrationInstance Build(IRegistration registration)
        {
            return new RegistrationInstance(typeof(Func<T>), new FactoryRegistration<T>(registration));
        }
    }
}