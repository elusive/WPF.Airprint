namespace WPF.Airprint.IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Terminal;
    using Xunit;

    public class TerminalServiceTests
    {
        private readonly ITerminalService _terminal;

        public TerminalServiceTests()
        {
            _terminal = new TerminalService();
        }

        [Fact]
        public async Task TestTerminalServiceExecuteAsyncForCreatePrintQueueCommandType()
        {
            // arrange
            const string ExistingContainer = "c5d87560a6f7";
            const string DeviceUri = "ipp://10.0.0.66:631/ipp/print";
            const string expected = "testqueue";

            // act
            var result = await _terminal.ExecuteAsync(CommandType.PrintServerAddQueue, expected, DeviceUri);

            // assert
            Assert.Contains(expected, string.Join(' ', result.StandardOutput.ToArray()));
        } 
    }
}
