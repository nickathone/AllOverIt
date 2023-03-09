namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record ShapeStyle
    {
        private static readonly ShapeStyle Default = new();

        public string Fill { get; set; }
        public string Stroke { get; set; }

        public bool IsDefault()
        {
            return this == Default;
        }
    }
}