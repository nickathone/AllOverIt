using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    public sealed class D2ErdGenerator : ErdGeneratorBase
    {
        private const string PrimaryKey = "primary_key";
        private const string ForeignKey = "foreign_key";

        private readonly ErdOptions _options;

        public D2ErdGenerator(ErdOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        public override string Generate(DbContext dbContext)
        {
            var sb = new StringBuilder();
            var relationships = new List<string>();

            var dbContextEntityTypes = dbContext.Model.GetEntityTypes();
            var entries = GetEntityColumnDescriptors(dbContext);

            var defaultShapeStyle = !_options.Entities.ShapeStyle.IsDefault()
                ? _options.Entities.ShapeStyle.AsText(2)
                : default;

            foreach (var (entityIdentifier, columns) in entries)
            {
                var entityName = entityIdentifier.TableName;

                sb.AppendLine($"{entityName}: {{");
                sb.AppendLine("  shape: sql_table");
                sb.AppendLine();

                var entityType = dbContextEntityTypes
                    .Single(entity => entity.ClrType == entityIdentifier.Type)
                    .ClrType;

                if (_options.TryGetEntityOptions(entityType, out var entityOptions))
                {
                    if (!entityOptions.ShapeStyle.IsDefault())
                    {
                        sb.AppendLine(entityOptions.ShapeStyle.AsText(2));
                        sb.AppendLine();
                    }
                }
                else
                {
                    if (defaultShapeStyle is not null)
                    {
                        sb.AppendLine(defaultShapeStyle);
                        sb.AppendLine();
                    }
                }

                foreach (var column in columns)
                {
                    var columnName = column.ColumnName;
                    var columnType = GetColumnDetail(column, _options);
                    var columnConstraint = GetColumnConstraint(column);

                    sb.AppendLine($"  {columnName}: {columnType} {columnConstraint}");

                    if (column.ForeignKeyPrincipals != null)
                    {
                        foreach (var foreignKey in column.ForeignKeyPrincipals)
                        {
                            var cardinality = GetColumnCardinality(foreignKey);

                            var relationship = cardinality.IsNullOrEmpty()
                                ? $"{foreignKey.EntityName}.{foreignKey.ColumnName} -> {entityName}.{columnName}"
                                : $"{foreignKey.EntityName}.{foreignKey.ColumnName} -> {entityName}.{columnName}: {cardinality}";

                            relationships.Add(relationship);
                        }
                    }
                }

                sb.AppendLine("}");
                sb.AppendLine();
            }

            foreach (var relationship in relationships)
            {
                sb.AppendLine(relationship);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GetColumnCardinality(PrincipalForeignKey foreignKey)
        {
            var cardinality = string.Empty;

            if (_options.Cardinality.LabelStyle.IsVisible)
            {
                cardinality = foreignKey.IsOneToMany
                    ? _options.Cardinality.OneToManyLabel.D2EscapeString()
                    : _options.Cardinality.OneToOneLabel.D2EscapeString();
            }

            if (cardinality.IsNullOrEmpty() || _options.Cardinality.LabelStyle.IsDefault())
            {
                return cardinality;
            }

            return $$"""
                   {{cardinality}} {
                   {{_options.Cardinality.LabelStyle.AsText(2)}}
                   }
                   """;
        }

        private static string GetColumnDetail(ColumnDescriptor column, ErdOptions configuration)
        {
            var columnType = column.MaxLength.HasValue && configuration.Entities.ShowMaxLength
                ? $"{column.ColumnType}({column.MaxLength})"
                : column.ColumnType;

            if (configuration.Entities.Nullable.IsVisible)
            {
                if (column.IsNullable && configuration.Entities.Nullable.Mode == NullableColumnMode.IsNull)
                {
                    columnType = $@"{columnType} {configuration.Entities.Nullable.IsNullLabel.D2EscapeString()}";
                }

                if (!column.IsNullable && configuration.Entities.Nullable.Mode == NullableColumnMode.NotNull)
                {
                    columnType = $@"{columnType} {configuration.Entities.Nullable.NotNullLabel.D2EscapeString()}";
                }
            }

            return columnType;
        }

        private static string GetColumnConstraint(ColumnDescriptor column)
        {
            return column.Constraint switch
            {
                ConstraintType.None => string.Empty,
                ConstraintType.PrimaryKey => $"{{ constraint: {PrimaryKey} }}",
                ConstraintType.ForeignKey => $"{{ constraint: {ForeignKey} }}",
                _ => throw new InvalidOperationException($"Unhandled constraint type '{column.Constraint}'.")
            };
        }
    }
}