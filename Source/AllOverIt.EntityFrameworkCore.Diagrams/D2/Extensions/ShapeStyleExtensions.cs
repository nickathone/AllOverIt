namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    public static class ShapeStyleExtensions
    {
        public static string AsText(this ShapeStyle shapeStyle, int indent)
        {
            return StyleStringBuilder.Create(indent, appender =>
            {
                if (shapeStyle.Fill != default)
                {
                    appender.Invoke("fill", shapeStyle.Fill);
                }

                if (shapeStyle.Stroke != default)
                {
                    appender.Invoke("stroke", shapeStyle.Stroke);
                }
            });
        }
    }
}