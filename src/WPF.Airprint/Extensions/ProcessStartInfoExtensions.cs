﻿namespace WPF.Airprint.Extensions
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
        /// <param name="taskName">Name of the task.</param>
        /// <param name="outputMessageDelegate">Handler for output.</param>
        /// <param name="errorMessageDelegate">Handler for error output.</param>
        /// <param name="processExitedCallback">Handler for process exit.</param>
        /// <param name="token">Cancel token.</param>
        /// <returns>Exit code result wrapped in a task.</returns>
        public static async Task<int> StartWithCancelAsync(
            [NotNull] this ProcessStartInfo processStartInfo,
            string taskName,
            Action<string> outputMessageDelegate,
            Action<string> errorMessageDelegate,
            Action<TerminalResult> processExitedCallback,
            CancellationToken token)
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
                    ps,
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

                // start process
                ps.Start();
                ps.BeginErrorReadLine();
                ps.BeginOutputReadLine();

                return await tcs.Task.ConfigureAwait(false);
            }
        }

        private static void DelegateLine(string line, Action<string> del)
        {
            del?.Invoke(string.IsNullOrWhiteSpace(line) ? line : line.TrimEnd());
        }
    }
}
