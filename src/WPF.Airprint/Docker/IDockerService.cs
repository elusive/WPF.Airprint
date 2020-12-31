using System.Threading.Tasks;

namespace WPF.Airprint.Docker
{
    public interface IDockerService
    {
        string PrintServerContainerUri { get; }

        Task<string> IsImageExisting(string imageName);
        Task<bool> IsContainerRunning(string containerUri);
        Task<bool> StartContainer(string containerImageId);
        Task BuildImage(string dockerTarGzPath);
    }
}
