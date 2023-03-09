namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public sealed record LabelStyle
    {
        private static readonly LabelStyle Default = new();

        public bool IsVisible { get; set; } = true;
        public int? FontSize { get; set; }
        public string FontColor { get; set; }
        public bool? Bold { get; set; }
        public bool? Underline { get; set; }
        public bool? Italic { get; set; }

        public bool IsDefault()
        {
            return this == Default;
        }
    }
}