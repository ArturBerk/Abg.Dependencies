﻿using System;
using System.Reflection;

namespace Abg.Dependencies
{
    public class TypeRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        public TypeRegistrationBuilder() : base(typeof(T))
        {
        }

        public override IRegistration Build()
        {
            var activator = new Activator();
            return IsTransient 
                ? new TransientFactoryRegistration<T>(activator.Activate, RegisterAs, OnActivateAction)
                : new SingleFactoryRegistration<T>(activator.Activate, RegisterAs, OnActivateAction,IsAutoActivate);
        }

        private class Activator
        {
            private readonly ConstructorInfo constructor;

            public Activator()
            {
                var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                if (constructors.Length == 0)
                    throw new Exception($"Type {typeof(T)}: Constructor not found");
                constructor = constructors[0];
            }

            public T Activate(IContainer container)
            {
                var parameters = constructor.GetParameters();
                var resolvedParameters = new object[parameters.Length];
                for (var index = 0; index < parameters.Length; index++)
                {
                    resolvedParameters[index] = container.Resolve(parameters[index].ParameterType);
                }

                return (T)constructor.Invoke(resolvedParameters);
            }
        }
    }
}