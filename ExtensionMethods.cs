using System.Collections.Generic;
using System.Linq;

namespace WatchLog
{
    public static class ExtensionMethods
    {
        public static IEnumerable<string> RemoveQuoteMarks(this IEnumerable<string> input) =>
            input.Select(x => x.RemoveQuoteMarks());

        public static string RemoveQuoteMarks(this string input) =>
            input.Length > 2
            && input.First() == '"'
            && input.Last() == '"' ? input.Substring(1, input.Length - 2) : input;

        public static IEnumerable<string> RemoveStartingSlash(this IEnumerable<string> input) =>
            input.Select(x => x.RemoveStartingSlash());

        public static string RemoveStartingSlash(this string input) =>
            input.Length > 1
            && input.First() == '\\' ? input.Substring(1, input.Length - 1) : input;
    }
}