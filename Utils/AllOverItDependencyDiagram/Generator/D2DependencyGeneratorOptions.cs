using System.Collections.Generic;

namespace AllOverItDependencyDiagram.Generator
{
    public sealed class D2DependencyGeneratorOptions
    {
        public int IndividualProjectTransitiveDepth { get; set; } = 1;
        public int AllProjectsTransitiveDepth { get; set; } = 0;
        public string PackageStyleFill { get; set; } = "#ADD8E6";
        public string TransitiveStyleFill { get; set; } = "#FFEC96";
        public IList<D2ImageFormat> ImageFormats { get; } = new List<D2ImageFormat> { D2ImageFormat.Svg, D2ImageFormat.Png };
        public string DiagramExportPath { get; set; }
    }
}