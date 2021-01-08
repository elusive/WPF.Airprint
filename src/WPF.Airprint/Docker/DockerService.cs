namespace WPF.Airprint.Docker
{
    using global::Docker.DotNet;
    using global::Docker.DotNet.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class DockerService : IDockerService
    {
        private readonly DockerClient _dockerClient;
        private string _containerId;

        public DockerService()
        {
            _dockerClient = new DockerClientConfiguration(new Uri(DockerUri())).CreateClient();
        }

        public string PrintServerContainerUri { get; private set; }

        //public async Task<bool> IsContainerRunning(string containerId)
        //{
        //    var filterEntry = new KeyValuePair<string, IDictionary<string, bool>>(
        //        "id", 
        //        new Dictionary<string, bool>(
        //            new[]
        //            {
        //                new KeyValuePair<string, bool>(containerId, true)
        //            }));
        //    var filters = new Dictionary<string, IDictionary<string, bool>>(new [] { filterEntry });
        //    var parameters = new ContainersListParameters() {Filters = filters};
        //    var containers = await _dockerClient.Containers.ListContainersAsync(parameters);
        //    return containers.Any();
        //}

        public async Task<bool> ExecuteCommand(string containerId, string[] command)
        {
            var p = new ContainerExecCreateParameters
            {
                Detach = false,
                Tty = true,
                Cmd = command,
            };
            var create = await _dockerClient.Exec.ExecCreateContainerAsync(containerId, p);

            if ( !string.IsNullOrEmpty(create?.ID))
            {
                var cancel = new CancellationTokenSource();
                cancel.CancelAfter(TimeSpan.FromSeconds(4));
                await _dockerClient.Exec.StartContainerExecAsync(create.ID, cancel.Token);
                return true;
            }

            return false;
        }

        public async Task<string> ExecuteCommandForOutput(string containerId, string[] command)
        {
            var p = new ContainerExecCreateParameters
            {
                Detach = false,
                AttachStderr = true,
                AttachStdout = true,
                Tty = true,
                Cmd = command,
            };
            var create = await _dockerClient.Exec.ExecCreateContainerAsync(containerId, p);

            if (!string.IsNullOrEmpty(create?.ID))
            {
                var cancel = new CancellationTokenSource();
                cancel.CancelAfter(TimeSpan.FromSeconds(8));
                var start = await _dockerClient.Exec.StartAndAttachContainerExecAsync(create.ID, true, cancel.Token);
                var output = await start?.ReadOutputToEndAsync(cancel.Token);
                return output.stdout;
            }

            return null;
        }

        private string DockerUri()  
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return
                (isWindows) ? Constants.DockerEngineWindowsUri :
                (isLinux) ? Constants.DockerEngineLinuxUri :
                throw new Exception(Constants.Messages.CannotDetermineOs);
        }

        public async Task BuildImage(string dockerfilePath)
        {
            if (!File.Exists(dockerfilePath))
            {
                throw new FileNotFoundException(Constants.Messages.DockerfileNotFound);
            }

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), dockerfilePath);
            var parameters = new ImageBuildParameters
            {
                 Dockerfile = "Dockerfile",
                 Target = Constants.DockerPrintServerImageName
            };
            await _dockerClient.Images.BuildImageFromDockerfileAsync(File.OpenRead(path), parameters);
        }

        public async Task<bool> StartContainer(string containerImageUri)
        {
            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = Constants.DockerPrintServerImageName,
                ExposedPorts = new Dictionary<string, EmptyStruct>(),
                Volumes = new Dictionary<string, EmptyStruct>()
            });

            _containerId = response.ID;

            return await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }

        public async Task DisposeAsync()
        {
            if (_containerId != null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());
            }
        }

        public async Task<string> IsImageExisting(string imageName)
        {
            var p = new ImagesListParameters
            {
                MatchName = Constants.DockerPrintServerImageName
            };

            var response = await _dockerClient.Images.ListImagesAsync(p);
            var firstResponse = response.FirstOrDefault();

            return firstResponse.ID;
        }

        public async Task<string> IsContainerExisting(string containerImageID)
        {
            var p = new ContainersListParameters() {All = true};

            var response = await _dockerClient.Containers.ListContainersAsync(p);
            var firstResponse = response.FirstOrDefault(c => c.ImageID.Contains(containerImageID));

            return firstResponse?.ID;
        }
    }
}
