using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Serato.Net.Services
{
    public sealed class FileWatcher
    {
        private readonly ILogger<FileWatcher> _logger;
        public event FileSystemEventHandlerAsync SessionFileChanged;
        public delegate Task FileSystemEventHandlerAsync(object sender, FileSystemEventArgs e);

        public FileWatcher(ILogger<FileWatcher> logger)
        {
            _logger = logger;
            SessionFileChanged += (obj, e) => Task.Run(() => _logger.LogDebug($"OnChanged: {e.Name}"));
            var fw = new FileSystemWatcher()
            {
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\_Serato_\History\Sessions",
                Filter = "*.session",
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.LastAccess
            };
            fw.Changed += OnChanged;

            fw.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            SessionFileChanged?.Invoke(sender, e);
        }
    }
}
