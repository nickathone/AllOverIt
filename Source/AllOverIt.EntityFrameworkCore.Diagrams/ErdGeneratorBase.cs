using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record EntityIdentifier(Type Type, string TableName);

    public abstract class ErdGeneratorBase : IErdGenerator
    {
        public abstract string Generate(DbContext dbContext);

        protected static IReadOnlyDictionary<EntityIdentifier, IEnumerable<ColumnDescriptor>> GetEntityColumnDescriptors(DbContext dbContext)
        {
            var entityDescriptors = new Dictionary<EntityIdentifier, IEnumerable<ColumnDescriptor>>();

            var entityTypes = dbContext.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var entityName = entityType.GetTableName();
                var descriptors = entityType.GetProperties().SelectAsReadOnlyCollection(ColumnDescriptor.Create);

                var identifier = new EntityIdentifier(entityType.ClrType, entityName);
                entityDescriptors.Add(identifier, descriptors);
            }

            return entityDescriptors;
        }
    }
}