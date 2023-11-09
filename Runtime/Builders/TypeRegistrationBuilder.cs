using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Abg.Dependencies
{
    internal class TypeRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        public TypeRegistrationBuilder() : base(typeof(T))
        {
        }

        public override IEnumerable<RegistrationInstance> Build()
        {
            var activator = new Activator();
            return BuildFrom(IsTransient
                ? new TransientRegistration<T>(activator.Activate, OnActivateAction)
                : new SingleRegistration<T>(activator.Activate, OnActivateAction, IsAutoActivate));
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
                    ParameterInfo parameterInfo = parameters[index];
                    if (container.TryResolve(parameterInfo.ParameterType, out var service))
                        resolvedParameters[index] = service;
                    else
                    {
                        var arg = parameterInfo.GetCustomAttribute<OptionalAttribute>();
                        if (arg == null)
                            container.Resolve(parameterInfo.ParameterType); // To throw exception
                        else
                            resolvedParameters[index] = default;
                    }
                }

                return (T)constructor.Invoke(resolvedParameters);
            }
        }
    }
}