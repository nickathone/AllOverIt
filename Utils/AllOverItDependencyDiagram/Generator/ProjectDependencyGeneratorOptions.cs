using System.Collections.Generic;

namespace AllOverItDependencyDiagram.Generator
{
    public sealed class ProjectDependencyGeneratorOptions
    {
        public int IndividualProjectTransitiveDepth { get; set; } = 1;

        public int AllProjectsTransitiveDepth { get; set; } = 0;

        public string PackageStyleFill { get; set; } = "#ADD8E6";

        public string TransitiveStyleFill { get; set; } = "#FFEC96";

        public IList<DiagramImageFormat> ImageFormats { get; } = new List<DiagramImageFormat> { DiagramImageFormat.Svg, DiagramImageFormat.Png };

        public string DiagramExportPath { get; set; }
    }
}