using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serato.Net.Services;
using Serato.Net.Tray.Stream;

namespace Serato.Net.Tray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private CancellationTokenSource cancellationTokenSrc;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            cancellationTokenSrc = new CancellationTokenSource();
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
            {
                services.AddSingleton(s => new FileWatcher(s.GetRequiredService<ILogger<FileWatcher>>()));
                services.AddSingleton(s => new SessionFileParser(s.GetRequiredService<ILogger<SessionFileParser>>(), s.GetRequiredService<FileWatcher>()));
                services.AddHostedService<SeratoStreamApi>();
            });
            Task.Run(() => host.Build().RunAsync(cancellationTokenSrc.Token), cancellationTokenSrc.Token);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            cancellationTokenSrc.Cancel();
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            cancellationTokenSrc.Dispose();
            base.OnExit(e);
        }
    }
}
