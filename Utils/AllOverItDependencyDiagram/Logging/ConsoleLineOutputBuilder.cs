using System;
using System.Collections.Generic;

namespace AllOverItDependencyDiagram.Logging
{
    internal sealed class ConsoleLineOutputBuilder
    {
        private readonly IList<ConsoleLineFragment> _fragments = new List<ConsoleLineFragment>();
        private readonly Action<ConsoleLineOutput> _logger;

        public ConsoleLineOutputBuilder(ConsoleColor consoleColor, string text, Action<ConsoleLineOutput> logger)
        {
            _logger = logger;

            AddFragment(consoleColor, text);
        }

        public ConsoleLineOutputBuilder AddFragment(ConsoleColor consoleColor, string text)
        {
            var fragment = new ConsoleLineFragment(consoleColor, text);

            _fragments.Add(fragment);

            return this;
        }

        public void Log(bool appendLineBreak)
        {
            var output = new ConsoleLineOutput(_fragments, appendLineBreak);

            _logger.Invoke(output);
        }
    }
}