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
    }
}
