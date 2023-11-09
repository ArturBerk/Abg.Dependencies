using System.Threading.Tasks;

namespace Abg.Dependencies
{
    public interface IInitializable
    {
        void Initialize();
    }
    
    public interface IAsyncInitializable
    {
        Task InitializeAsync();
    }
}