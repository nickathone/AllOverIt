using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public static class ErdFormatter
    {
        public static TFormatter Create<TFormatter>(Action<ErdOptions> configure = default) where TFormatter : ErdFormatterBase
        {
            var options = new ErdOptions();

            configure?.Invoke(options);

            var formatter = (TFormatter) Activator.CreateInstance(typeof(TFormatter), new[] { options });

            return formatter;
        }
    }
}