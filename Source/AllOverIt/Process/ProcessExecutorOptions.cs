using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AllOverIt.Process
{
    public sealed record ProcessExecutorOptions
    {
        public string ProcessFileName { get; }
        public string WorkingDirectory { get; internal set; }
        public string Arguments { get; internal set; }
        public TimeSpan Timeout { get; internal set; }
        public IDictionary<string, string> EnvironmentVariables { get; internal set; }
        public DataReceivedEventHandler StandardOutputHandler {get; internal set;}
        public DataReceivedEventHandler ErrorOutputHandler { get; internal set; }

        public ProcessExecutorOptions(string processFileName)
        {
            ProcessFileName = processFileName.WhenNotNullOrEmpty(nameof(processFileName));
        }
    }
}
