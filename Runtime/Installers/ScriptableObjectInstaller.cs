using UnityEngine;

namespace Abg.Dependencies
{
    public abstract class ScriptableObjectInstaller : ScriptableObject, IDependencyInstaller
    {
        public abstract void Install(ContainerBuilder builder);
    }
}