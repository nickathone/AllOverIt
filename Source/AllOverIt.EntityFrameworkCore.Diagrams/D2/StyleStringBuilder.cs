using System;
using System.Text;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    public static class StyleStringBuilder
    {
        public static string Create(int indent, Action<Action<string, string>> action)
        {
            var indenting = new string(' ', indent);
            var builder = new StringBuilder();

            void appender(string key, string value)
            {
                builder.AppendLine($"{indenting}{indenting}{key}: {value}");
            }

            action.Invoke(appender);

            var styleText = builder.ToString().TrimEnd();

            return $$"""
                   {{indenting}}style: {
                   {{styleText.D2EscapeString()}}
                   {{indenting}}}
                   """;
        }
    }
}