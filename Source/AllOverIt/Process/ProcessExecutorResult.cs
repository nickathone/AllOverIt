using System;

namespace AllOverIt.Process
{
    public class ProcessExecutorResult
    {
        public DateTime StartTime { get; }
        public DateTime ExitTime { get; }
        public int ExitCode { get; }
        public TimeSpan RunTime { get; }

        internal ProcessExecutorResult(System.Diagnostics.Process process)
        {
            StartTime = process.StartTime;
            ExitTime = process.ExitTime;
            ExitCode = process.ExitCode;
            RunTime = ExitTime - StartTime;
        }
    }
}
