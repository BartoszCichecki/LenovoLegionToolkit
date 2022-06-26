using System;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class StringExtensions
    {
        public static string GetUntilOrEmpty(this string text, string stopAt)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);
            if (charLocation > 0)
                return text[..charLocation];

            return string.Empty;
        }

        public static string GetAfterOrEmpty(this string text, string stopAt)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var charLocation = text.IndexOf(stopAt, StringComparison.Ordinal) + 1;
            if (charLocation <= text.Length)
                return text[charLocation..];

            return string.Empty;
        }
    }
}
