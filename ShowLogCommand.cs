using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;

namespace WatchLog
{
    [Command]
    public class ShowLogCommand : ICommand
    {
        [CommandParameter(0, Description = "Command to repeatedly execute")]
        public string Command { get; set; }

        [CommandParameter(1, Description = "Arguments to provide to the command")]
        public List<string> Arguments { get; set; }

        [CommandOption("interval", 'n', Description = "Logarithm base.")]
        public double ExecutionInterval { get; set; } = 2;

        public ValueTask ExecuteAsync(IConsole console)
        {
            Arguments = Arguments.RemoveQuoteMarks().ToList();

            console.Output.WriteLine($"{Command} {string.Join(' ', Arguments)}");

            // Return empty task because our command executes synchronously
            return default;
        }
    }
}