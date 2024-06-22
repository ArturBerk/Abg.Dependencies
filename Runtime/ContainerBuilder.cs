using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public class ContainerBuilder
    {
        private readonly List<IRegistrationBuilder> builders = new List<IRegistrationBuilder>();

        public IRegistrationBuilder<T> RegisterInstance<T>(T instance)
        {
            var builder = new InstanceRegistrationBuilder<T>(instance);
            builders.Add(builder);
            return builder;
        }

        public IRegistrationBuilder RegisterType(Type type)
        {
            var builder = (IRegistrationBuilder) Activator.CreateInstance(
                typeof(TypeRegistrationBuilder<>).MakeGenericType(type));
            builders.Add(builder);
            return builder;
        }

        public IRegistrationBuilder<T> RegisterType<T>()
        {
            var builder = new TypeRegistrationBuilder<T>();
            builders.Add(builder);
            return builder;
        }

        public IRegistrationBuilder<T> RegisterFactory<T>(Func<IContainer, T> factory)
        {
            var builder = new FactoryRegistrationBuilder<T>(factory);
            builders.Add(builder);
            return builder;
        }

        public IContainer Build(IContainer parent)
        {
            var registrations = new List<RegistrationInstance>();
            for (int i = 0; i < builders.Count; i++)
            {
                registrations.AddRange(builders[i].Build());
            }

            return new Container(parent, registrations);
        }
    }

    public interface IRegistration
    {
        object Resolve(IContainer container);
        void Activate(IContainer container);
    }

    public readonly struct ResolvedInstance<T>
    {
        public readonly IContainer Container;
        public readonly T Instance;

        public ResolvedInstance(IContainer container, T instance)
        {
            Container = container;
            Instance = instance;
        }
    }

    public readonly struct RegistrationInstance
    {
        public readonly Type Type;
        public readonly IRegistration Registration;

        public RegistrationInstance(Type type, IRegistration registration)
        {
            Type = type;
            Registration = registration;
        }
    }
}