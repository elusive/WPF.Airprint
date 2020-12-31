using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WPF.Airprint.Terminal
{

    public interface ITerminalService
    {
        ProcessStartInfo CreateCmdStartInfo(string cmd);

        Task<TerminalResult> ExecuteAsync(string cmd);

        Task<TerminalResult> ExecuteAsync(ProcessStartInfo processStartInfo, List<string> standardOutput, List<string> standardError, CancellationToken cancellationToken = new CancellationToken());
    }
}
