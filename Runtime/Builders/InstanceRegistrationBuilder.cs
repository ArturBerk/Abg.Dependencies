namespace Abg.Dependencies
{
    public class InstanceRegistrationBuilder<T> : RegistrationBuilderBase<T>
    {
        private readonly T instance;

        public InstanceRegistrationBuilder(T instance) : base(instance.GetType())
        {
            this.instance = instance;
        }

        public override IRegistration Build()
        {
            return new InstanceRegistration<T>(instance, RegisterAs, OnActivateAction, IsAutoActivate);
        }
    }
}