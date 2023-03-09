using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public abstract class ErdGeneratorBase : IErdGenerator
    {
        public abstract string Generate(DbContext dbContext);

        protected static IReadOnlyDictionary<string, IEnumerable<ColumnDescriptor>> GetEntityColumnDescriptors(DbContext dbContext)
        {
            var entityDescriptors = new Dictionary<string, IEnumerable<ColumnDescriptor>>();

            var entityTypes = dbContext.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var entityName = entityType.DisplayName();
                var descriptors = entityType.GetProperties().SelectAsReadOnlyCollection(ColumnDescriptor.Create);

                entityDescriptors.Add(entityName, descriptors);
            }

            return entityDescriptors;
        }
    }
}