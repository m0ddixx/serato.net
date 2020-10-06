using System;
using System.IO;

namespace Serato.Net.Services
{
    public sealed class FileWatcher
    {
        private static readonly Lazy<FileWatcher> Lazy = new Lazy<FileWatcher>(() => new FileWatcher());
        public static FileWatcher Instance => Lazy.Value;
        public event FileSystemEventHandler SessionFileChanged;

        private FileWatcher()
        {
            var fw = new FileSystemWatcher()
            {
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\_Serato_\History\Sessions",
                Filter = "*.session",
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite
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
