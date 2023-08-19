using System;

namespace Abg.Dependencies
{
    public interface IRegistrationBuilder
    {
        Type Type { get; }
        IRegistration Build();
    }

    public interface IRegistrationBuilder<T> : IRegistrationBuilder
    {
        IRegistrationBuilder<T> As<T1>();
        IRegistrationBuilder<T> As(Type type);
        IRegistrationBuilder<T> Transient();
        IRegistrationBuilder<T> Single();
        IRegistrationBuilder<T> AutoActivate();
        IRegistrationBuilder<T> OnActivated(Action<ResolvedInstance<T>> onActivate);
    }
}