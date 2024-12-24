using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public interface IRegistrationBuilder
    {
        Type Type { get; }
        IEnumerable<RegistrationInstance> Build();
    }

    public interface IRegistrationBuilder<T> : IRegistrationBuilder
    {
        IRegistrationBuilder<T> As<T1>();
        IRegistrationBuilder<T> As(Type type);
        IRegistrationBuilder<T> Transient();
        IRegistrationBuilder<T> Single();
        IRegistrationBuilder<T> AutoActivate();
        IRegistrationBuilder<T> OnActivated(Action<ResolvedInstance<T>> onActivate);
        IRegistrationBuilder<T> WithFactory<T1>();
        IRegistrationBuilder<T> WithFactory();
    }
}