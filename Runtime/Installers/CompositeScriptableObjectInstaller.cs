using UnityEngine;

namespace Abg.Dependencies
{
    public class CompositeScriptableObjectInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private ScriptableObjectInstaller[] scriptableObjectInstallers;
        [SerializeField] private MonoBehaviourInstaller[] monoBehaviourInstallers;
        
        public override void Install(ContainerBuilder builder)
        {
            if (scriptableObjectInstallers != null)
            {
                foreach (ScriptableObjectInstaller installer in scriptableObjectInstallers)
                {
                    installer.Install(builder);
                }
            }
            if (monoBehaviourInstallers != null)
            {
                foreach (MonoBehaviourInstaller installer in monoBehaviourInstallers)
                {
                    installer.Install(builder);
                }
            }
        }
    }
}