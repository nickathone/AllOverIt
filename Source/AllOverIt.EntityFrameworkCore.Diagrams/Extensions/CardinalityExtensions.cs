using AllOverIt.Assertion;
using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Extensions
{
    public static class CardinalityExtensions
    {
        public static void SetLabelStyle(this ErdOptions.CardinalityOptions cardinality, Action<LabelStyle> configure)
        {
            _ = configure.WhenNotNull(nameof(configure));

            configure.Invoke(cardinality.LabelStyle);
        }
    }
}