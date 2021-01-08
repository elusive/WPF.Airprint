namespace WPF.Airprint.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Terminal;

    public static class ProcessStartInfoExtensions
    {
        public const int WaitForExitMilliseconds = 500;

        /// <summary>
        ///     Starts a process with support for cancellation via the
        ///     provided token. Console output for the process is redirected
        ///     to the provided delegates.
        /// </summary>
        /// <param name="processStartInfo"><see cref="ProcessStartInfo" /> instance.</param>
        /// <param name="processExitedCallback">Handler for process exit.</param>
        /// <param name="token">Cancel token.</param>
        /// <param name="timeoutMs">Milliseconds used to timeout waiting for exit.</param>
        /// <returns>Exit code result wrapped in a task.</returns>
        public static async Task<int> StartWithTimeoutAsync(
            [NotNull] this ProcessStartInfo processStartInfo,
            Action<TerminalResult> processExitedCallback,
            CancellationToken token,
            int timeoutMs = 3500)
        {
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            var standardOut = new List<string>();
            var standardErr = new List<string>();

            var tcs = new TaskCompletionSource<int>();
            var ps = new Process { StartInfo = processStartInfo, EnableRaisingEvents = true };
            ps.Exited += (obj, args) =>
            {
                ps.WaitForExit();
                var exitCode = ps.ExitCode;
                var result = new TerminalResult(
                    exitCode == 0 ? TerminalResultEnum.Success : TerminalResultEnum.Failure,
                    exitCode,
                    standardOut,
                    standardErr);
                processExitedCallback?.Invoke(result);

                ps.CancelErrorRead();
                ps.CancelOutputRead();
                ps.Dispose();
                tcs.TrySetResult(exitCode);
            };

            // register cancel handling
            await using (token.Register(() =>
            {
                tcs.TrySetCanceled();
                try
                {
                    if (ps.HasExited) return;

                    ps.Kill();
                    ps.WaitForExit(WaitForExitMilliseconds);
                }
                finally
                {
                    ps.Dispose();
                }
            }))
            {
                // in case canceled before starting
                if (token.IsCancellationRequested) throw new OperationCanceledException(token);

                // bind output and error handling
                ps.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data)) standardOut.Add(args.Data);
                };
                ps.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data)) standardErr.Add(args.Data);
                };

                // start process in a thread that we can join
                ps.Start();
                ps.BeginErrorReadLine();
                ps.BeginOutputReadLine();
                ps.WaitForExit(timeoutMs);

                // check to see if still running after timeout
                if (!ps.HasExited)
                { 
                    ps.Kill(true);
                }

                return await tcs.Task.ConfigureAwait(false);
            }
        }
    }
}
