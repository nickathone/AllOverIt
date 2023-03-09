using AllOverIt.EntityFrameworkCore.Diagrams;
using AllOverIt.EntityFrameworkCore.Diagrams.D2;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.EntityFrameworkCore.Diagrams.Extensions;
using D2ErdGenerator.Data;
using D2ErdGenerator.Data.Entities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace D2ErdGenerator
{
    internal class Program
    {
        static async Task Main()
        {
            // Defaults are:
            // * Show non-nullable columns as [NOT NULL]
            // * Show MaxLength
            // * Show 1:1 cardinality as "ONE-TO-ONE"
            // * Show 1:N cardinality as "ONE-TO-MANY"
            // * Cardinality label style is as per D2 defaults
            var erdFormatter = ErdGenerator
                .Create<AllOverIt.EntityFrameworkCore.Diagrams.D2.D2ErdGenerator>(options =>
                {
                    options.Entity.Nullable.IsVisible = true;
                    options.Entity.Nullable.Mode = NullableColumnMode.NotNull;

                    // config.Entity.IsNullLabel = ...
                    // config.Entity.NotNullLabel = ...;

                    // This is the default
                    options.Entity.ShowMaxLength = true;

                    // Selectively style an entity - based on the table name (which may not be the same as the class name)
                    options.Entity[nameof(AuthorBlog)].SetShapeStyle(style =>
                    {
                        style.Fill = "black";
                        style.Stroke = "#E8F4FF";   // very pale blue
                    });

                    // Individual properties
                    options.Cardinality.LabelStyle.FontSize = 18;

                    // .. or via this extension method
                    options.Cardinality.SetLabelStyle(style =>
                    {
                        style.FontSize = 18;
                        style.FontColor = "blue";   // "yellow";
                        style.Bold = true;
                        // style.Underline = false;
                        // style.Italic = false;
                    });

                    // config.Cardinality.OneToOneLabel = ...;
                    // config.Cardinality.OneToManyLabel = ...;
                });

            var dbContext = new AppDbContext();

            // This generates the diagram as text
            var erd = erdFormatter.Generate(dbContext);
            Console.WriteLine(erd);

            // This generates the diagram, saves it as a text file and exports to SVG, PNG, PDF
            var exportOptions = new D2ErdExportOptions
            {
                DiagramFileName = "sample_erd.d2",
                LayoutEngine = "elk",
                //Theme = Theme.DarkMauve,
                Formats = new[] { ExportFormat.Svg, ExportFormat.Png, ExportFormat.Pdf },
                StandardOutputHandler = LogOutput,
                ErrorOutputHandler = LogOutput          // Note: d2.exe seems to log everything to the error output
            };

            await erdFormatter.ExportAsync(dbContext, exportOptions);

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }

        private static void LogOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                Console.WriteLine(e.Data);
            }
        }
    }
}