using AllOverIt.Assertion;
using AllOverIt.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AllOverIt.Process.Extensions
{
    public static class ProcessExecutorOptionsExtensions
    {
        public static ProcessExecutorOptions WithWorkingDirectory(this ProcessExecutorOptions options, string workingDirectory)
        {
            return options with { WorkingDirectory = workingDirectory };
        }

        public static ProcessExecutorOptions WithArguments(this ProcessExecutorOptions options, params string[] arguments)
        {
            _ = arguments.WhenNotNull(nameof(arguments));

            return options.WithArguments(arguments.ToArray(), true);
        }

        public static ProcessExecutorOptions WithArguments(this ProcessExecutorOptions options, IEnumerable<string> arguments, bool escape = true)
        {
            _ = arguments.WhenNotNull(nameof(arguments));

            var stringBuilder = new StringBuilder();

            foreach (var argument in arguments)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ');
                }

                stringBuilder.Append(escape ? Argument.Escape(argument) : argument);
            }

            return options with { Arguments = stringBuilder.ToString() };
        }

        public static ProcessExecutorOptions WithTimeout(this ProcessExecutorOptions options, double milliseconds)
        {
            return options with { Timeout = TimeSpan.FromMilliseconds(milliseconds) };
        }

        public static ProcessExecutorOptions WithTimeout(this ProcessExecutorOptions options, TimeSpan timeout)
        {
            return options with { Timeout = timeout };
        }

        public static ProcessExecutorOptions WithEnvironmentVariables(this ProcessExecutorOptions options, IDictionary<string, string> environmentVariables)
        {
            _ = environmentVariables.WhenNotNull(nameof(environmentVariables));

            return options with { EnvironmentVariables = environmentVariables };
        }

        public static ProcessExecutorOptions WithEnvironmentVariables(this ProcessExecutorOptions options, Action<IDictionary<string, string>> variables)
        {
            _ = variables.WhenNotNull(nameof(variables));

            // Supports chaining multiple calls together
            var environmentVariables = options.EnvironmentVariables ?? new Dictionary<string, string>();

            variables.Invoke(environmentVariables);

            return options.WithEnvironmentVariables(environmentVariables);
        }

        public static ProcessExecutorOptions WithStandardOutputHandler(this ProcessExecutorOptions options, DataReceivedEventHandler standardOutputHandler)
        {
            return options with { StandardOutputHandler = standardOutputHandler };
        }

        public static ProcessExecutorOptions WithErrorOutputHandler(this ProcessExecutorOptions options, DataReceivedEventHandler errorOutputHandler)
        {
            return options with { ErrorOutputHandler = errorOutputHandler };
        }

        public static IProcessExecutor BuildProcessExecutor(this ProcessExecutorOptions options)
        {
            return new ProcessExecutor(options);
        }
    }
}
