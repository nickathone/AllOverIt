using AllOverIt.Assertion;
using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Extensions
{
    public static class EntityOptionsExtensions
    {
        public static void SetShapeStyle(this ErdOptions.EntityOptions options, Action<ShapeStyle> configure)
        {
            _ = configure.WhenNotNull(nameof(configure));

            configure.Invoke(options.ShapeStyle);
        }

        public static void SetShapeStyle(this ErdOptions.EntityOptions options, ShapeStyle shapeStyle)
        {
            _ = shapeStyle.WhenNotNull(nameof(shapeStyle));

            options.ShapeStyle = shapeStyle;
        }
    }
}