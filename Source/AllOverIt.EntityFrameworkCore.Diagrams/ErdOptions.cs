namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record ErdOptions
    {
        private const string DefaultOneToOneLabel = "ONE-TO-ONE";
        private const string DefaultOneToManyLabel = "ONE-TO-MANY";
        private const string DefaultIsNullLabel = @"\[NULL\]";
        private const string DefaultNotNullLabel = @"\[NOT NULL\]";

        public sealed class NullableColumn
        {
            public bool IsVisible { get; set; }
            public NullableColumnMode Mode { get; set; }
            public string IsNullLabel { get; set; } = DefaultIsNullLabel;
            public string NotNullLabel { get; set; } = DefaultNotNullLabel;
        }

        public sealed class EntityOptions
        {
            public NullableColumn Nullable { get; } = new();
            public bool ShowMaxLength { get; set; } = true;
        }

        public sealed class CardinalityOptions
        {
            public LabelStyle LabelStyle { get; } = new();
            public string OneToOneLabel { get; set; } = DefaultOneToOneLabel;
            public string OneToManyLabel { get; set; } = DefaultOneToManyLabel;
        }

        public EntityOptions Entity { get; } = new();
        public CardinalityOptions Cardinality { get; } = new();
    }
}