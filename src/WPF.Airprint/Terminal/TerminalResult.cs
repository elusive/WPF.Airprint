namespace WPF.Airprint.Terminal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    public sealed class TerminalResult
    {
        private const string ToStringFormat = @"Terminal process [{0}] result was: {1}";

        public TerminalResult(
            TerminalResultEnum result, 
            int exitCode, 
            IEnumerable<string> standardOut, 
            IEnumerable<string> standardErr)
        {
            Result = result;
            ExitCode = exitCode;
            StandardError = standardErr;
            StandardOutput = standardOut;
        }

        public IEnumerable<string> StandardOutput { get; }

        public IEnumerable<string> StandardError { get; }

        public TerminalResultEnum Result { get; }

        public int ExitCode { get; }

        public override string ToString()
        {
            return string.Format(ToStringFormat, "Terminal Process", Result);
        }
    }


    public enum TerminalResultEnum
    {
        Unknown,
        Success,
        Failure
    }
}
    