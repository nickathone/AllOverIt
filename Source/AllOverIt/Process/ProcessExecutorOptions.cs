using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Process
{
    [ExcludeFromCodeCoverage]
    public sealed record ProcessExecutorOptions
    {
        public string ProcessFileName { get; }
        public string WorkingDirectory { get; init; }
        public string Arguments { get; init; }
        public TimeSpan Timeout { get; init; }
        public IDictionary<string, string> EnvironmentVariables { get; init; }
        public DataReceivedEventHandler StandardOutputHandler {get; init; }
        public DataReceivedEventHandler ErrorOutputHandler { get; init; }

        public ProcessExecutorOptions(string processFileName)
        {
            ProcessFileName = processFileName.WhenNotNullOrEmpty(nameof(processFileName));
        }
    }
}
