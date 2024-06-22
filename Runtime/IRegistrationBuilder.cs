using System;
using System.Collections.Generic;

namespace Abg.Dependencies
{
    public interface IRegistrationBuilder
    {
        Type Type { get; }
        IEnumerable<RegistrationInstance> Build();
        
        IRegistrationBuilder As<T1>();
        IRegistrationBuilder As(Type type);
        IRegistrationBuilder Transient();
        IRegistrationBuilder Single();
        IRegistrationBuilder AutoActivate();
        IRegistrationBuilder WithFactory<T1>();
        IRegistrationBuilder WithFactory();
    }

    public interface IRegistrationBuilder<T> : IRegistrationBuilder
    {
        new IRegistrationBuilder<T> As<T1>();
        new IRegistrationBuilder<T> As(Type type);
        new IRegistrationBuilder<T> Transient();
        new IRegistrationBuilder<T> Single();
        new IRegistrationBuilder<T> AutoActivate();
        new IRegistrationBuilder<T> OnActivated(Action<ResolvedInstance<T>> onActivate);
        new IRegistrationBuilder<T> WithFactory<T1>();
        new IRegistrationBuilder<T> WithFactory();
    }
}