using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Io;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    public sealed class D2ErdFormatter : ErdFormatterBase
    {
        private const string PrimaryKey = "primary_key";
        private const string ForeignKey = "foreign_key";

        private readonly ErdOptions _options;

        public D2ErdFormatter(ErdOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        public override string Generate(DbContext dbContext)
        {
            var sb = new StringBuilder();
            var relationships = new List<string>();

            var entries = GetEntityColumnDescriptors(dbContext);

            foreach (var (entityName, columns) in entries)
            {
                sb.AppendLine($"{entityName}: {{");
                sb.AppendLine("  shape: sql_table");
                sb.AppendLine();

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
                    ? _options.Cardinality.OneToManyLabel
                    : _options.Cardinality.OneToOneLabel;
            }

            if (cardinality.IsNullOrEmpty() || _options.Cardinality.LabelStyle.IsDefault())
            {
                return cardinality;
            }

            return $"{cardinality} {_options.Cardinality.LabelStyle.AsText()}";
        }

        private static string GetColumnDetail(ColumnDescriptor column, ErdOptions configuration)
        {
            var columnType = column.MaxLength.HasValue && configuration.Entity.ShowMaxLength
                ? $"{column.ColumnType}({column.MaxLength})"
                : column.ColumnType;

            if (configuration.Entity.Nullable.IsVisible)
            {
                if (column.IsNullable && configuration.Entity.Nullable.Mode == NullableColumnMode.IsNull)
                {
                    columnType = $@"{columnType} {configuration.Entity.Nullable.IsNullLabel}";
                }

                if (!column.IsNullable && configuration.Entity.Nullable.Mode == NullableColumnMode.NotNull)
                {
                    columnType = $@"{columnType} {configuration.Entity.Nullable.NotNullLabel}";
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