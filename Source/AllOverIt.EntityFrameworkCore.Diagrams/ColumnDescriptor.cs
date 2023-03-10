using AllOverIt.Assertion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed class ColumnDescriptor
    {
        public string ColumnName { get; }
        public string ColumnType { get; }
        public bool IsNullable { get; }
        public int? MaxLength { get; }
        public ConstraintType Constraint { get; } = ConstraintType.None;
        public IReadOnlyCollection<PrincipalForeignKey> ForeignKeyPrincipals { get; }

        public ColumnDescriptor(IProperty column)
        {
            ColumnName = column.Name;
            ColumnType = column.GetColumnType();
            IsNullable = column.IsColumnNullable();

            var maxLength = column.GetAnnotations().SingleOrDefault(annotation => annotation.Name == nameof(MaxLength));

            if (maxLength is not null)
            {
                MaxLength = (int) maxLength.Value;
            }

            if (column.IsPrimaryKey())
            {
                Constraint = ConstraintType.PrimaryKey;
            }
            else if (column.IsForeignKey())
            {
                Constraint = ConstraintType.ForeignKey;
                ForeignKeyPrincipals = GetForeignKeys(column);
            }
        }

        // Alternative factory method that can be used as a method group
        public static ColumnDescriptor Create(IProperty column)
        {
            return new ColumnDescriptor(column);
        }

        private static IReadOnlyCollection<PrincipalForeignKey> GetForeignKeys(IProperty column)
        {
            var foreignKeys = new List<PrincipalForeignKey>();

            foreach (var foreignKey in column.GetContainingForeignKeys())
            {
                var principalEntity = foreignKey.PrincipalEntityType;

                var parentToChildNavigation = foreignKey.DependentToPrincipal?.Inverse;

                Throw<InvalidOperationException>.WhenNull(
                    parentToChildNavigation,
                    $"A parent to child navigation property exists between {principalEntity.DisplayName()} and {column.DeclaringEntityType.DisplayName()}, but not the reverse.");

                var isOneToMany = parentToChildNavigation.IsCollection;

                // TODO: Configure / handle composite keys
                var entityColumn = new PrincipalForeignKey
                {
                    EntityName = principalEntity.GetTableName(),
                    ColumnName = string.Join(", ", foreignKey.PrincipalKey.Properties.Select(property => property.Name)),
                    IsOneToMany = isOneToMany
                };

                foreignKeys.Add(entityColumn);
            }

            return foreignKeys;
        }
    }
}