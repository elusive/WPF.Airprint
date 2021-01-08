namespace WPF.Airprint.IntegrationTests.Docker
{
    using System.Linq;
    using WPF.Airprint.Docker;
    using Xunit;

    public class DockerServiceTests
    {
        private readonly IDockerService _docker;

        public DockerServiceTests()
        {
            _docker = new DockerService();
        }


        [Fact] 
        public async void TestIsImageExistingMethodReturnsTrueForExistingImage()
        {
            // arrange
            const string expectedName = Constants.DockerPrintServerImageName;

            // act
            var response = await _docker.IsImageExisting(expectedName);
            var actual = response ?? string.Empty;

            // assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async void TestIsContainerExistingMethodReturnsTrueForExistingContainer()
        {
            // arrange
            const bool expected = true;
            const string containerId = "c5d87560a6f7";

            // act
            var response = await _docker.IsContainerExisting(containerName: containerId);

            // assert
            Assert.True(expected);
        }

        [Fact]
        public async void TestExecuteCommandInContainer()
        {
            // arrange
            const string queueName = "testqueue";
            const string uri = "ipp://10.0.0.66:631/ipp/print";
            string[] command = new[]
            {
                "lpadmin",
                "-p",
                queueName,
                "-v",
                uri,
                "-E",
                "-m",
                "everywhere"
            };

            // act
            var resp = await _docker.ExecuteCommand("c3d4", command);

            // assert 
            Assert.True(resp);
        }

        [Fact]
        public async void TestExecuteCommandForOutput()
        {
            // arrange
            const string queueName = "testqueue";
            string[] command = new[]
            {
                "lpstat",
                "-a",
                queueName,
            };

            // act
            var resp = await _docker.ExecuteCommandForOutput("c3d4", command);

            // assert 
            Assert.NotEmpty(resp);
        }
    }
}
