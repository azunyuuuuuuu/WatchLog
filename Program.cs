using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Cocona;

namespace WatchLog
{
    public class Program : CoconaLiteConsoleAppBase
    {
        public static async Task Main(string[] args)
            => await CoconaLiteApp.RunAsync<Program>(args);

        public async Task LogCommand(
            [Argument] string command,
            [Argument] string[] arguments,
            [Option] double delay = 2.0)
        {
            var temp = arguments.RemoveQuoteMarks().RemoveStartingSlash();

            Console.WriteLine($"{command} {string.Join(' ', temp)}");

            var buffers = new Buffers(
                oldbuffer: string.Empty,
                newbuffer: string.Empty);

            while (!Context.CancellationToken.IsCancellationRequested)
                buffers = await ExecuteCommandInLoopAsync(command, temp, buffers, delay);
        }

        private async Task<Buffers> ExecuteCommandInLoopAsync(
            string command,
            IEnumerable<string> arguments,
            Buffers buffers,
            double delay)
        {
            var cli = Cli.Wrap(command);

            if (arguments.Count() > 0)
                cli = cli.WithArguments(arguments, false);

            var result = await cli.ExecuteBufferedAsync();

            var newbuffers = buffers with
            {
                oldbuffer = buffers.newbuffer,
                newbuffer = result.StandardOutput
            };

            OutputDifferences(newbuffers);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delay), Context.CancellationToken);
            }
            catch (TaskCanceledException) { }

            return newbuffers;
        }

        private record Buffers(string oldbuffer, string newbuffer);

        private void OutputDifferences(Buffers buffers)
        {
            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var tempnew = buffers.newbuffer.Split(Environment.NewLine);
            var tempold = buffers.oldbuffer.Split(Environment.NewLine);
            var tempadded = tempnew.Where(x => !tempold.Contains(x));
            var tempremoved = tempold.Where(x => !tempnew.Contains(x));

            foreach (var line in tempadded)
                Console.WriteLine($"{timestamp} \u001b[32m[ADD]\u001b[0m: {line}");
            foreach (var line in tempremoved)
                Console.WriteLine($"{timestamp} \u001b[31m[REM]\u001b[0m: {line}");
        }

    }
}
