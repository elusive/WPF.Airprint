namespace WPF.Airprint.Terminal
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;

    public class TerminalService : ITerminalService
    {
        private const int _defaultTimeoutMilliseconds = 6500;
        private CancellationTokenSource _tokenSource;

        public TerminalService()
        {
            _tokenSource = new CancellationTokenSource();

            CommandTimeoutMilliseconds = _defaultTimeoutMilliseconds;
        }

        public ProcessStartInfo CreateCmdStartInfo(string cmd)
        {
            var psi = new ProcessStartInfo("cmd.exe", "/C " + cmd);
            return psi;
        }

        public int CommandTimeoutMilliseconds { get; set; }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public void CancelAfter(int milliseconds)
        {
            _tokenSource.CancelAfter(TimeSpan.FromMilliseconds(milliseconds));
        }

        public async Task<TerminalResult> ExecuteAsync(string commandText)
        {
            return await ExecuteAsync(CommandStrings.CommandExe, commandText, _tokenSource.Token);
        }

        public async Task<TerminalResult> ExecuteAsync(CommandType type, params object[] args)
        {
            var (cmd, txt) = ResolveCommandText(type, args);
            return await ExecuteAsync(cmd, txt, _tokenSource.Token);
        }

        /// <summary>
        /// Executes command at the terminal asynchronously.
        /// </summary>
        /// <param name="program">The program to execute.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<TerminalResult> ExecuteAsync(
            string program,
            string commandText,
            CancellationToken cancellationToken)
        {
            var processStartInfo = new ProcessStartInfo(program, commandText);

            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;

            TerminalResult result = null;
            
            await processStartInfo.StartWithTimeoutAsync((res) =>
            {
                result = res;
            }, cancellationToken);

            return result;
        }

        private Tuple<string, string> ResolveCommandText(CommandType type, params object[] args)
        {
            switch (type)
            {
                case CommandType.GetIpAddress:
                    return new Tuple<string, string>(CommandStrings.DndSdExe, string.Format(CommandStrings.GetIpAddressFormat, args));

                case CommandType.PrintServerAddQueue:
                    return new Tuple<string, string>(CommandStrings.DockerExe, string.Format(CommandStrings.AddPrintQueue, args));

                case CommandType.PrintServerGetQueueStatus:
                    return new Tuple<string, string>(CommandStrings.DockerExe, string.Format(CommandStrings.GetPrintQueueStatus, args));

                case CommandType.PrintServerPrintFile:
                    return new Tuple<string, string>(CommandStrings.DockerExe, string.Format(CommandStrings.PrintFile, args));
                
                default: throw new InvalidOperationException("Unknown command type specified.");
            }
        }
    
    }
}
