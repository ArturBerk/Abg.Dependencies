using System.Collections.Generic;

namespace Abg.Dependencies
{
    internal class InstanceRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        private readonly T instance;

        public InstanceRegistrationBuilder(T instance) : base(instance.GetType())
        {
            this.instance = instance;
            IsAutoActivate = true;
            if (instance.GetType() != typeof(T))
                As<T>();
        }

        public override IEnumerable<RegistrationInstance> Build()
        {
            return BuildFrom(new InstanceRegistration<T>(instance, OnActivateAction, IsAutoActivate));
        }
    }
}