using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliWrap;
using CliWrap.Buffered;

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

        public async ValueTask ExecuteAsync(IConsole console)
        {
            Arguments = Arguments.RemoveQuoteMarks().ToList();

            console.Output.WriteLine($"{Command} {string.Join(' ', Arguments)}");

            while (true)
            {
                var command = Cli.Wrap(Command);

                if (Arguments.Count > 0)
                    command = command.WithArguments(Arguments, false);

                var result = await command.ExecuteBufferedAsync();


                await console.Output.WriteAsync(result.StandardOutput);

                await Task.Delay(TimeSpan.FromSeconds(ExecutionInterval));
            }

            // Return empty task because our command executes synchronously
            // return Task.CompletedTask;
        }
    }
}