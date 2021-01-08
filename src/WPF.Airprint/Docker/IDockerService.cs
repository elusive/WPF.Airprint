using System.Threading.Tasks;

namespace WPF.Airprint.Docker
{
    public interface IDockerService
    {
        string PrintServerContainerUri { get; }

        Task<string> IsContainerExisting(string containerName);
        Task<string> IsImageExisting(string imageName);
        Task<bool> StartContainer(string containerImageId);
        Task BuildImage(string dockerTarGzPath);

        Task<bool> ExecuteCommand(string containerId, string[] command);
        Task<string> ExecuteCommandForOutput(string containerId, string[] command);
    }
}
