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

        private string _oldbuffer = string.Empty;
        private string _newbuffer = string.Empty;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            Arguments = Arguments.RemoveQuoteMarks().ToList();

            console.Output.WriteLine($"{Command} {string.Join(' ', Arguments)}");

            while (true)
                await ExecuteCommandInLoopAsync(console);
        }

        private async Task ExecuteCommandInLoopAsync(IConsole console)
        {
            var command = Cli.Wrap(Command);

            if (Arguments.Count > 0)
                command = command.WithArguments(Arguments, false);

            var result = await command.ExecuteBufferedAsync();
            _oldbuffer = _newbuffer;
            _newbuffer = result.StandardOutput;

            await OutputDifferences(console);

            await Task.Delay(TimeSpan.FromSeconds(ExecutionInterval));
        }

        private async Task OutputDifferences(IConsole console)
        {
            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var tempnew = _newbuffer.Split(Environment.NewLine);
            var tempold = _oldbuffer.Split(Environment.NewLine);
            var tempadded = tempnew.Where(x => !tempold.Contains(x));
            var tempremoved = tempold.Where(x => !tempnew.Contains(x));

            foreach (var line in tempadded)
                await console.Output.WriteLineAsync($"{timestamp} \u001b[32m[ADD]\u001b[0m: {line}");
            foreach (var line in tempremoved)
                await console.Output.WriteLineAsync($"{timestamp} \u001b[31m[REM]\u001b[0m: {line}");
        }
    }
}