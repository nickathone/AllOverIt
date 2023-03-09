namespace AllOverIt.Process
{
    public sealed class ProcessExecutorBufferedResult : ProcessExecutorResult
    {
        public string StandardOutput { get; }
        public string ErrorOutput { get; }

        internal ProcessExecutorBufferedResult(System.Diagnostics.Process process, string standardOutput, string errorOutput)
            : base(process)
        {
            StandardOutput = standardOutput;
            ErrorOutput = errorOutput;
        }
    }
}
