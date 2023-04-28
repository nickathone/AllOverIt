using System.Collections.Generic;

namespace AllOverItDependencyDiagram.Generator
{
    public sealed class ProjectDependencyGeneratorOptions
    {
        public int IndividualProjectTransitiveDepth { get; set; } = 1;

        public int AllProjectsTransitiveDepth { get; set; } = 0;

        public string PackageStyleFill { get; set; } = "#ADD8E6";

        public string TransitiveStyleFill { get; set; } = "#FFEC96";

        // Also supports svg and pdf
        public IList<DiagramImageFormat> ImageFormats { get; } = new List<DiagramImageFormat> { DiagramImageFormat.Png };

        public string ExportPath { get; set; }
    }
}