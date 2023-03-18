using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Process.Exceptions;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemProcess = System.Diagnostics.Process;

namespace AllOverIt.Process
{
    public sealed class ProcessExecutor : IProcessExecutor
    {
        [ExcludeFromCodeCoverage]
        private sealed class DataOutputBuffer
        {
            private readonly StringBuilder _dataOutput = new();

            public void OnDataReceived(object sender, DataReceivedEventArgs eventArgs)
            {
                if (eventArgs.Data is not null)
                {
                    _dataOutput.AppendLine(eventArgs.Data);
                }
            }

            public override string ToString()
            {
                return _dataOutput.ToString();
            }
        }

        internal SystemProcess _process = new();
        internal readonly ProcessExecutorOptions _options;

        public ProcessExecutor(ProcessExecutorOptions options)
        {
            _options = options.WhenNotNull(nameof(options));

            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.WorkingDirectory = _options.WorkingDirectory;
            _process.StartInfo.FileName = _options.ProcessFileName;
            _process.StartInfo.Arguments = _options.Arguments;

            if (_options.EnvironmentVariables.IsNotNullOrEmpty())
            {
                foreach (var (key, value) in _options.EnvironmentVariables)
                {
                    _process.StartInfo.EnvironmentVariables.Add(key, value);
                }
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task<ProcessExecutorResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await DoExecuteAsync(_options.StandardOutputHandler, _options.ErrorOutputHandler, cancellationToken);

            return new ProcessExecutorResult(_process);
        }

        [ExcludeFromCodeCoverage]
        public async Task<ProcessExecutorBufferedResult> ExecuteBufferedAsync(CancellationToken cancellationToken = default)
        {
            var standardOutput = new DataOutputBuffer();
            var errorOutput = new DataOutputBuffer();

            await DoExecuteAsync(standardOutput.OnDataReceived, errorOutput.OnDataReceived, cancellationToken);

            return new ProcessExecutorBufferedResult(_process, standardOutput.ToString(), errorOutput.ToString());
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            if (_process is not null)
            {
                if (_options.StandardOutputHandler is not null)
                {
                    _process.OutputDataReceived -= _options.StandardOutputHandler;
                }

                if (_options.ErrorOutputHandler is not null)
                {
                    _process.ErrorDataReceived -= _options.ErrorOutputHandler;
                }

                _process.Dispose();
            }

            _process = null;
        }

        [ExcludeFromCodeCoverage]
        private async Task DoExecuteAsync(DataReceivedEventHandler standardOutputHandler, DataReceivedEventHandler errorOutputHandler,
            CancellationToken cancellationToken = default)
        {
            if (standardOutputHandler is not null)
            {
                _process.OutputDataReceived += standardOutputHandler;
            }

            if (errorOutputHandler is not null)
            {
                _process.ErrorDataReceived += errorOutputHandler;
            }

            _process.Start();

            if (standardOutputHandler is not null)
            {
                _process.BeginOutputReadLine();
            }

            if (errorOutputHandler is not null)
            {
                _process.BeginErrorReadLine();
            }

            Exception processException = null;

            try
            {
                // Cater for an explicit timeout for these scenarios
                // - Less than NET 5 is used (there's no WaitForExitAsync() method)
                // - The provided cancellationToken does not have an associated timeout (via a CancellationTokenSource)
                //                var milliseconds = (int)_options.Timeout.TotalMilliseconds;

                //                if (milliseconds != 0)        // -1 means indefinite
                //                {
                //#if NET5_0_OR_GREATER
                //                    using (var cts = new CancellationTokenSource(milliseconds))
                //                    {
                //                        using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
                //                        {
                //                            await WaitForProcessAsync(linked.Token).ConfigureAwait(false);
                //                        }
                //                    }
                //#else
                //                    Throw<ProcessException>.When(milliseconds == 0, "A timeout must be specified when using the NET STANDARD target.");

                //                    await WaitForProcessAsync(milliseconds).ConfigureAwait(false);
                //#endif
                //                }
                //                else
                //                {
                //#if NET5_0_OR_GREATER
                //                    await WaitForProcessAsync(cancellationToken).ConfigureAwait(false);
                //#else
                //                    await WaitForProcessAsync(-1).ConfigureAwait(false);
                //#endif
                //                }

                var milliseconds = (int) _options.Timeout.TotalMilliseconds;

#if NET5_0_OR_GREATER
                // Cater for an explicit timeout for the scenario where the provided cancellationToken does not have an associated timeout (via a CancellationTokenSource)
                if (milliseconds != 0)        // -1 means indefinite
                {
                    using (var cts = new CancellationTokenSource(milliseconds))
                    {
                        using (var linked = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
                        {
                            await WaitForProcessAsync(linked.Token).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    await WaitForProcessAsync(cancellationToken).ConfigureAwait(false);
                }
#else
                Throw<ProcessException>.When(milliseconds == 0, "A timeout must be specified when using the NETSTANDARD2_1 target.");

                await WaitForProcessAsync(milliseconds).ConfigureAwait(false);
#endif
            }
            catch (OperationCanceledException)
            {
                KillProcess();

                throw new TimeoutException("Process wait timeout expired.");
            }
            catch (Exception exception)
            {
                processException = exception;
            }
            finally
            {
                if (standardOutputHandler is not null)
                {
                    _process.CancelOutputRead();
                    _process.OutputDataReceived -= standardOutputHandler;
                }

                if (errorOutputHandler is not null)
                {
                    _process.CancelErrorRead();
                    _process.ErrorDataReceived -= errorOutputHandler;
                }
            }

            if (processException != null)
            {
                throw new ProcessException("Process execution failed.", processException);
            }
        }

#if NET5_0_OR_GREATER
        [ExcludeFromCodeCoverage]
        private Task WaitForProcessAsync(CancellationToken cancellationToken)
        {
            return _process.WaitForExitAsync(cancellationToken);
        }
#else
        [ExcludeFromCodeCoverage]
        private Task WaitForProcessAsync(int milliseconds)
        {
            _process.WaitForExit(milliseconds);

            return Task.CompletedTask;
        }
#endif

        [ExcludeFromCodeCoverage]
        private void KillProcess()
        {
#if NETSTANDARD2_1
            _process.Kill();
#else
            _process.Kill(true);
#endif
        }
    }
}
