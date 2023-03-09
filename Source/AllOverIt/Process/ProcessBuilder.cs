namespace AllOverIt.Process
{
    public static class ProcessBuilder
    {
        public static ProcessExecutorOptions For(string processFilename)
        {
            return new ProcessExecutorOptions(processFilename);
        }
    }
}
