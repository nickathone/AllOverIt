using System.Collections.Generic;
using System.Diagnostics;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    public sealed class D2ErdExportOptions
    {
        private const string DefaultLayoutEngine = "elk";
        public const Theme DefaultTheme = Theme.Neutral;

        public string DiagramFileName { get; init; }
        public Theme Theme { get; init; } = DefaultTheme;
        public string LayoutEngine { get; init; } = DefaultLayoutEngine;
        public IEnumerable<ExportFormat> Formats { get; init; } // Skipped if null / empty
        public DataReceivedEventHandler StandardOutputHandler { get; init; }
        public DataReceivedEventHandler ErrorOutputHandler { get; init; }
    }
}