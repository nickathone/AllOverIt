using System;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record ErdOptions
    {
        private const string DefaultOneToOneLabel = "ONE-TO-ONE";
        private const string DefaultOneToManyLabel = "ONE-TO-MANY";
        private const string DefaultIsNullLabel = "[NULL]";
        private const string DefaultNotNullLabel = "[NOT NULL]";

        private readonly IDictionary<Type, EntityOptions> _entityOptions = new Dictionary<Type, EntityOptions>();

        public sealed class NullableColumn
        {
            public bool IsVisible { get; set; }
            public NullableColumnMode Mode { get; set; }
            public string IsNullLabel { get; set; } = DefaultIsNullLabel;
            public string NotNullLabel { get; set; } = DefaultNotNullLabel;
        }

        public sealed class EntityOptions
        {
            public ShapeStyle ShapeStyle { get; internal set; } = new();
        }

        public sealed class EntityGlobalOptions
        {
            public NullableColumn Nullable { get; } = new();
            public bool ShowMaxLength { get; set; } = true;
        }

        public sealed class CardinalityOptions
        {
            public LabelStyle LabelStyle { get; internal set; } = new();
            public string OneToOneLabel { get; set; } = DefaultOneToOneLabel;
            public string OneToManyLabel { get; set; } = DefaultOneToManyLabel;
        }

        public EntityGlobalOptions Entities { get; } = new();
        public CardinalityOptions Cardinality { get; } = new();

        public EntityOptions Entity<TEntity>() where TEntity : class
        {
            return GetEntityOptions(typeof(TEntity));
        }

        internal bool TryGetEntityOptions(Type entity, out EntityOptions options)
        {
            return _entityOptions.TryGetValue(entity, out options);
        }

        private EntityOptions GetEntityOptions(Type entity)
        {
            if (!_entityOptions.TryGetValue(entity, out var entityByNameOptions))
            {
                entityByNameOptions = new EntityOptions();
                _entityOptions[entity] = entityByNameOptions;
            }

            return entityByNameOptions;
        }
    }
}