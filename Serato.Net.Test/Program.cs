using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serato.Net.Services;

namespace Serato.Net.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(new FileWatcher());
                    services.AddSingleton(new SessionFileParser(true));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                });
            await builder.RunConsoleAsync();
        }

        private static void InstanceOnSessionFileChanged(object? sender, EventArgs e)
        {

        }
    }
}
