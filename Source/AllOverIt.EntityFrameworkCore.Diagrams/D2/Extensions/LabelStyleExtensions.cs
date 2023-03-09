namespace AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions
{
    public static class LabelStyleExtensions
    {
        public static string AsText(this LabelStyle labelStyle, int indent)
        {
            return StyleStringBuilder.Create(indent, appender =>
            {
                if (labelStyle.FontSize != default)
                {
                    appender.Invoke("font-size", $"{labelStyle.FontSize}");
                }

                if (labelStyle.FontColor != default)
                {
                    appender.Invoke("font-color", labelStyle.FontColor);
                }

                if (labelStyle.Bold.HasValue)
                {
                    appender.Invoke("bold", $"{GetBoolString(labelStyle.Bold.Value)}");
                }

                if (labelStyle.Underline.HasValue != default)
                {
                    appender.Invoke("underline", $"{GetBoolString(labelStyle.Underline.Value)}");
                }

                if (labelStyle.Italic.HasValue != default)
                {
                    appender.Invoke("italic", $"{GetBoolString(labelStyle.Italic.Value)}");
                }
            });
        }

        private static string GetBoolString(bool value)
        {
            return value ? "true" : "false";
        }
    }
}