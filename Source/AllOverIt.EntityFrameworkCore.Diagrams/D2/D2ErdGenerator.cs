using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    public sealed class D2ErdGenerator : ErdGeneratorBase
    {
        private readonly ErdOptions _options;

        public D2ErdGenerator(ErdOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        public override string GenerateDiagram(DbContext dbContext, IReadOnlyDictionary<EntityIdentifier, IReadOnlyCollection<ColumnDescriptor>> entries)
        {
            var sb = new StringBuilder();
            var relationships = new List<string>();

            var dbContextEntityTypes = dbContext.Model.GetEntityTypes().AsReadOnlyCollection();

            var defaultShapeStyle = !_options.Entities.ShapeStyle.IsDefault()
                ? _options.Entities.ShapeStyle.AsText(2)
                : default;

            var entityNodeGenerator = new EntityNodeGenerator(_options, dbContextEntityTypes, defaultShapeStyle);

            foreach (var (entityIdentifier, columns) in entries)
            {
                var entityNode = entityNodeGenerator.CreateNode(entityIdentifier, columns, relationships.Add);

                sb.AppendLine(entityNode);
                sb.AppendLine();
            }

            foreach (var relationship in relationships)
            {
                sb.AppendLine(relationship);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}