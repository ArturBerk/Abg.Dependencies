using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Abg.Dependencies
{
    public class ProjectContext
    {
        private static IContainer container;

        public static IContainer Container
        {
            get
            {
                if (container == null)
                    Initialize();
                return container;
            }
        }

        public static async ValueTask DisposeAsync()
        {
            if (container == null) return;
            try
            {
                await container.DisposeAsync();
            }
            finally
            {
                container = null;
            }
        }

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        // private static void Reset()
        // {
        //     var disposePart = DisposeAsync();
        //     Task.Run(async () =>
        //     {
        //         await disposePart;
        //     }).Wait();
        // }

        public static void Initialize(IDependencyInstaller installer = null)
        {
            var projectContainerBuilder = new ContainerBuilder();
            var pdi = Resources.Load<ScriptableObjectInstaller>("ProjectDependencies");
            if (pdi != null)
                pdi.Install(projectContainerBuilder);
            if (installer != null)
                installer.Install(projectContainerBuilder);
            container = projectContainerBuilder.Build(null);
        }
    }
}