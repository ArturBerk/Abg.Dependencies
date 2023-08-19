using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Abg.Dependencies
{
    public static class Extensions
    {
        public static void Install(this ContainerBuilder builder, Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                builder.Install(root);
            }
        }

        public static void Install(this ContainerBuilder builder, GameObject root)
        {
            foreach (IDependencyInstaller installer in root.GetComponents<IDependencyInstaller>())
            {
                try
                {
                    installer.Install(builder);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public static IRegistrationBuilder<T> WithInjectMethod<T>(
            this IRegistrationBuilder<T> builder)
        {
            return builder.OnActivated(a => a.Container.InjectMethod(a.Instance));
        }

        public static void InjectMethod(this IContainer context, object instance)
        {
            var methods = instance.GetType().GetMethods(BindingFlags.Instance
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic);

            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<AutoInject>() == null) continue;
                var parameters = methodInfo.GetParameters();
                var parameterValues = new object[parameters.Length];
                for (var index = 0; index < parameters.Length; index++)
                {
                    ParameterInfo parameterInfo = parameters[index];
                    parameterValues[index] = context.Resolve(parameterInfo.ParameterType);
                }

                methodInfo.Invoke(instance, parameterValues);
                break;
            }
        }

        public static IContainer CreateScope(this IContainer container, Action<ContainerBuilder> build)
        {
            var builder = new ContainerBuilder();
            build(builder);
            return builder.Build(container);
        }

        public static IRegistrationBuilder<T> AsImplementedInterfaces<T>(this IRegistrationBuilder<T> builder)
        {
            var type = builder.Type;
            while (type != null)
            {
                foreach (Type @interface in type.GetInterfaces())
                {
                    builder.As(@interface);
                }
                type = type.BaseType;
            }
            
            return builder;
        }

        public static void Inject(this IContainer context, object instance)
        {
            InjectProperties(context, instance);
            InjectFields(context, instance);
        }

        public static void InjectProperties(this IContainer context, object instance)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            var type = instance.GetType();
            foreach (var propertyInfo in type.GetRuntimeProperties().Where(pi => pi.CanWrite))
            {
                var propertyType = propertyInfo.PropertyType;
                var injectAttribute = propertyInfo.GetCustomAttribute<AutoInject>();
                if (injectAttribute != null
                    && (!propertyType.GetTypeInfo().IsValueType || propertyType.GetTypeInfo().IsEnum)
                    && (!propertyType.IsArray || !propertyType.GetElementType().GetTypeInfo().IsValueType)
                    && propertyInfo.GetIndexParameters().Length == 0)
                {
                    propertyInfo.SetValue(instance, context.Resolve(propertyType), null);
                    
                    // propertyInfo.SetValue(instance, injectAttribute.Key != null
                    //     ? context.ResolveKeyed(injectAttribute.Key, propertyType)
                    //     : context.Resolve(propertyType), null);
                }
            }
        }

        public static void InjectFields(this IContainer context, object instance)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            var type = instance.GetType();
            foreach (var fieldInfo in type.GetRuntimeFields())
            {
                var propertyType = fieldInfo.FieldType;
                var injectAttribute = fieldInfo.GetCustomAttribute<AutoInject>();
                if (injectAttribute != null
                    && (!propertyType.GetTypeInfo().IsValueType || propertyType.GetTypeInfo().IsEnum)
                    && (!propertyType.IsArray || !propertyType.GetElementType().GetTypeInfo().IsValueType))
                {
                    fieldInfo.SetValue(instance, context.Resolve(propertyType));
                    
                    // fieldInfo.SetValue(instance, injectAttribute.Key != null
                    //     ? context.ResolveKeyed(injectAttribute.Key, propertyType)
                    //     : context.Resolve(propertyType));
                }
            }
        }
    }
}