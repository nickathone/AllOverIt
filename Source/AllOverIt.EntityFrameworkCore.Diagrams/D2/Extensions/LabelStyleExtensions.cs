using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    public static class LabelStyleExtensions
    {
        public static string AsText(this LabelStyle labelStyle)
        {
            var builder = new StringBuilder();

            if (labelStyle.FontSize != default)
            {
                builder.AppendLine($"    font-size: {labelStyle.FontSize}");
            }

            if (labelStyle.FontColor != default)
            {
                builder.AppendLine($"    font-color: {labelStyle.FontColor}");
            }

            if (labelStyle.Bold.HasValue)
            {
                builder.AppendLine($"    bold: {GetBoolString(labelStyle.Bold.Value)}");
            }

            if (labelStyle.Underline.HasValue)
            {
                builder.AppendLine($"    underline: {GetBoolString(labelStyle.Underline.Value)}");
            }

            if (labelStyle.Italic.HasValue)
            {
                builder.AppendLine($"    italic: {GetBoolString(labelStyle.Italic.Value)}");
            }

            var style = builder.ToString().TrimEnd();

            return $$"""
                   {
                     style: {
                   {{style}}
                     }
                   }
                   """;
        }

        private static string GetBoolString(bool value)
        {
            return value ? "true" : "false";
        }
    }
}