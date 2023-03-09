using System;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    public static class ErdGenerator
    {
        public static TFormatter Create<TFormatter>(Action<ErdOptions> configure = default) where TFormatter : ErdGeneratorBase
        {
            var options = new ErdOptions();

            configure?.Invoke(options);

            var formatter = (TFormatter) Activator.CreateInstance(typeof(TFormatter), new[] { options });

            return formatter;
        }
    }
}