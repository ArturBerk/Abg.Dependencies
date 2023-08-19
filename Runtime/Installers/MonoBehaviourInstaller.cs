using UnityEngine;

namespace Abg.Dependencies
{
    public abstract class MonoBehaviourInstaller : MonoBehaviour, IDependencyInstaller
    {
        public abstract void Install(ContainerBuilder builder);
    }
}