using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Io;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    public static class D2ErdFormatterExtensions
    {
        // Exports the diagram to a text file (options.DiagramFileName) as a minimum, and optionally additional
        // formats (options.Formats).
        public static async Task ExportAsync(this D2ErdFormatter formatter, DbContext dbContext, D2ErdExportOptions options,
            CancellationToken cancellationToken = default)
        {
            _ = dbContext.WhenNotNull(nameof(dbContext));
            _ = options.WhenNotNull(nameof(options));

            var content = formatter.Generate(dbContext);

            await CreateDiagramFileAsync(content, options, cancellationToken).ConfigureAwait(false);
            await FormatDiagramFileAsync(options, cancellationToken).ConfigureAwait(false);
            await CreateAdditionalFormatsAsync(options, cancellationToken).ConfigureAwait(false);
        }

        private static Task CreateDiagramFileAsync(string content, D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            return FileUtils.CreateFileWithContentAsync(content, options.DiagramFileName, cancellationToken);
        }

        private static Task FormatDiagramFileAsync(D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            var formatFile = ProcessBuilder
               .For("d2.exe")
               .WithArguments("fmt", options.DiagramFileName)
               .WithStandardOutputHandler(options.StandardOutputHandler)        // null is OK
               .WithErrorOutputHandler(options.ErrorOutputHandler)              // null is OK
               .BuildProcessExecutor();

            return formatFile.ExecuteAsync(cancellationToken);
        }

        private static async Task CreateAdditionalFormatsAsync(D2ErdExportOptions options, CancellationToken cancellationToken)
        {
            if (options.Formats.IsNullOrEmpty())
            {
                return;
            }

            var layoutEngine = options.LayoutEngine.ToLowerInvariant();

            foreach (var format in options.Formats)
            {
                var extension = $"{format}".ToLowerInvariant();
                var exportFileName = Path.ChangeExtension(options.DiagramFileName, extension);

                var export = ProcessBuilder
                   .For("d2.exe")
                   .WithArguments("-l", layoutEngine, "-t", $"{(int)options.Theme}", options.DiagramFileName, exportFileName)
                   .WithStandardOutputHandler(options.StandardOutputHandler)    // null is OK
                   .WithErrorOutputHandler(options.ErrorOutputHandler)          // null is OK
                   .BuildProcessExecutor();

                await export.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}