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
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddCommandLine(args);
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(s => new FileWatcher(s.GetRequiredService<ILogger<FileWatcher>>()));
                    services.AddSingleton(s => new SessionFileParser(s.GetRequiredService<ILogger<SessionFileParser>>(), s.GetRequiredService<FileWatcher>()));
                    services.AddHostedService<SeratoTestService>();
                });
            await builder.RunConsoleAsync();
        }

        private static void InstanceOnSessionFileChanged(object? sender, EventArgs e)
        {

        }
    }    
}
