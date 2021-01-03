using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WPF.Airprint.Terminal
{

    public interface ITerminalService
    {
        int CommandTimeoutMilliseconds { get; set; }

        void Cancel();

        void CancelAfter(int milliseconds);

        ProcessStartInfo CreateCmdStartInfo(string commandText);

        Task<TerminalResult> ExecuteAsync(CommandType type, params object[] args);

        Task<TerminalResult> ExecuteAsync(string program, string commandText, CancellationToken token);
    }
}
