using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record ErdOptions
    {
        private const string DefaultOneToOneLabel = "ONE-TO-ONE";
        private const string DefaultOneToManyLabel = "ONE-TO-MANY";
        private const string DefaultIsNullLabel = "[NULL]";
        private const string DefaultNotNullLabel = "[NOT NULL]";

        public sealed class NullableColumn
        {
            public bool IsVisible { get; set; }
            public NullableColumnMode Mode { get; set; }
            public string IsNullLabel { get; set; } = DefaultIsNullLabel;
            public string NotNullLabel { get; set; } = DefaultNotNullLabel;
        }

        public sealed class EntityByNameOptions
        {
            public ShapeStyle ShapeStyle { get; } = new();
        }

        public sealed class EntityGlobalOptions
        {
            private readonly IDictionary<string, EntityByNameOptions> _byNameOptions = new Dictionary<string, EntityByNameOptions>();

            public NullableColumn Nullable { get; } = new();
            public bool ShowMaxLength { get; set; } = true;
            public EntityByNameOptions this[string name] => GetEntityByNameOptions(name);

            internal bool TryGetEntityByNameOptions(string tableName, out EntityByNameOptions options)
            {
                return _byNameOptions.TryGetValue(tableName, out options);
            }

            private EntityByNameOptions GetEntityByNameOptions(string tableName)
            {
                if (!_byNameOptions.TryGetValue(tableName, out var entityByNameOptions))
                {
                    entityByNameOptions = new EntityByNameOptions();
                    _byNameOptions[tableName] = entityByNameOptions;
                }

                return entityByNameOptions;
            }
        }

        public sealed class CardinalityOptions
        {
            public LabelStyle LabelStyle { get; } = new();
            public string OneToOneLabel { get; set; } = DefaultOneToOneLabel;
            public string OneToManyLabel { get; set; } = DefaultOneToManyLabel;
        }

        public EntityGlobalOptions Entity { get; } = new();
        public CardinalityOptions Cardinality { get; } = new();
    }
}