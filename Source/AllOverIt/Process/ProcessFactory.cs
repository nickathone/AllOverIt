using AllOverIt.Extensions;
using System.Diagnostics.CodeAnalysis;
using SystemProcess = System.Diagnostics.Process;

namespace AllOverIt.Process
{
    internal static class ProcessFactory
    {
        [ExcludeFromCodeCoverage]
        public static SystemProcess CreateProcess(ProcessExecutorOptions options)
        {
            var process = new SystemProcess();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = options.WorkingDirectory;
            process.StartInfo.FileName = options.ProcessFileName;
            process.StartInfo.Arguments = options.Arguments;

            if (options.EnvironmentVariables.IsNotNullOrEmpty())
            {
                foreach (var (key, value) in options.EnvironmentVariables)
                {
                    process.StartInfo.EnvironmentVariables.Add(key, value);
                }
            }

            return process;
        }
    }
}
