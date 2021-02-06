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
    }
}