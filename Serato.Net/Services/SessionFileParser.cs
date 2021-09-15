using Serato.Net.Structs;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serato.Net.Util;
using Microsoft.Extensions.Logging;

namespace Serato.Net.Services
{
    public sealed class SessionFileParser : IDisposable, IAsyncDisposable
    {
        public event FinishedParsingTracksEventHandler? FinishedParsingTracks;
        private readonly Mutex _mut;
        private FileStream? _stream;
        private BinaryReader? _reader;
        private readonly List<AdatStruct> _adatList;

        private readonly IEnumerable<FieldPropertiesAttribute> _attributes =
            (IEnumerable<FieldPropertiesAttribute>)typeof(TrackInfo).GetCustomAttributes(
                typeof(FieldPropertiesAttribute), false);

        private readonly ILogger<SessionFileParser> _logger;

        public SessionFileParser(ILogger<SessionFileParser> logger, FileWatcher fileWatcher) : this(logger, fileWatcher, true)
        {

        }
        public SessionFileParser(ILogger<SessionFileParser> logger, FileWatcher fileWatcher, bool bindEvents = true)
        {
            _logger = logger;
            _mut = new Mutex();
            _adatList = new List<AdatStruct>();
            Initialize(fileWatcher, bindEvents);
        }

        private void Initialize(FileWatcher fileWatcher, bool bindEvents = true)
        {
            if (bindEvents)
                fileWatcher.SessionFileChanged += async (sender, e) => await ParseSessionFile(sender, e);
        }

        private async Task ParseSessionFile(object sender, FileSystemEventArgs e)
        {
            _mut.WaitOne();
            try
            {
                _stream = File.OpenRead(e.FullPath);
                _stream.Position = 0;
                _reader = new BinaryReader(_stream);
                await ParseFile();
                var tracks = ProcessTrackInfo();
                await OnFinishedParsingTracks(new FinishedParsingTracksEventArgs(tracks, e.Name));
            }
            catch (IOException exception)
            {
                _logger.Log(LogLevel.Error, exception.ToString());
                Console.WriteLine(exception);
            }
            finally
            {
                _adatList?.Clear();
                _reader?.Close();
                _stream?.Close();
                _mut.ReleaseMutex();
            }
        }

        private async Task ParseFile()
        {
            if (_reader == null)
                throw new InvalidOperationException($"{nameof(_reader)} is null");
            while (_reader.BaseStream.Length - _reader.BaseStream.Position >= 8)
            {
                await ParseHeaderWithChunk();
            }
        }

        private async Task ParseHeaderWithChunk()
        {
            if (_reader == null)
                throw new InvalidOperationException($"{nameof(_reader)} is null");
            var header = new ChunkHeader()
            {
                Identifier = Encoding.UTF8.GetString(_reader.ReadBytes(4)),
                Length = BinaryPrimitives.ReadInt32BigEndian(_reader.ReadBytes(4))
            };
            await ParseChunk(header);
        }

        private async Task ParseChunk(ChunkHeader header)
        {
            Task parserTask = header.Identifier switch
            {
                "vrsn" => ParseVrsnChunk(header.Length),
                "oent" => ParseHeaderWithChunk(),
                "adat" => ParseAdatChunk(header.Length),
                _ => Task.CompletedTask,
            };
            await parserTask;
        }

        public Task ParseVrsnChunk(int length)
        {
            return Task.CompletedTask;
        }

        public Task ParseAdatChunk(int length)
        {
            if (_reader == null)
                throw new InvalidOperationException($"{nameof(_reader)} is null");
            _adatList?.Add(new AdatStruct()
            {
                Data = _reader.ReadBytes(length)
            });
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<TrackInfo> ProcessTrackInfo()
        {
            foreach (var adat in _adatList)
            {
                var ti = new TrackInfo();
                await using var mem = new MemoryStream(adat.Data);
                using var r = new BinaryReader(mem);
                while (r.PeekChar() > -1)
                {
                    var id = BinaryPrimitives.ReadInt32BigEndian(r.ReadBytes(4));
                    var length = BinaryPrimitives.ReadInt32BigEndian(r.ReadBytes(4));
                    var data = r.ReadBytes(length);
                    await ParseField(id, data, ref ti);
                }
                _logger.Log(LogLevel.Information, ti.ToString());
                yield return ti;
            }
        }

        private Task ParseField(int id, byte[] data, ref TrackInfo ti)
        {
            var prop = typeof(TrackInfo).GetProperties()
                .FirstOrDefault(p => (p.CustomAttributes?.Any(c =>
                        c.AttributeType == typeof(FieldPropertiesAttribute) &&
                        (c.ConstructorArguments[0].Value is int argId && argId == id)))
                    .GetValueOrDefault());
            if (prop == null) return Task.CompletedTask;
            var t = prop.PropertyType;
            if (t == typeof(int))
            {
                if (data.Length == 1)
                {
                    prop.SetValue(ti, (int)data[0]);
                }
                else
                {
                    prop.SetValue(ti, BinaryPrimitives.ReadInt32BigEndian(data));
                }
            }
            else if (t == typeof(string))
            {
                prop.SetValue(ti, Encoding.BigEndianUnicode.GetString(data).Replace("\0", string.Empty));
            }
            else if (t == typeof(DateTimeOffset))
            {
                prop.SetValue(ti, DateTimeOffset.FromUnixTimeSeconds(BinaryPrimitives.ReadInt64BigEndian(data)));
            }
            else if (t == typeof(TimeSpan))
            {
                prop.SetValue(ti, TimeSpan.FromSeconds(BinaryPrimitives.ReadInt64BigEndian(data)));
            }

            return Task.CompletedTask;
        }

        private async Task OnFinishedParsingTracks(FinishedParsingTracksEventArgs e)
        {
            if(FinishedParsingTracks != null)
                await FinishedParsingTracks(this, e);
        }

        public delegate Task FinishedParsingTracksEventHandler(object sender, FinishedParsingTracksEventArgs e);

        public void Dispose()
        {
            _mut.Dispose();
            _stream?.Dispose();
            _reader?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_stream != null)
                await _stream.DisposeAsync();
            _mut.Dispose();
            _reader?.Dispose();
        }
    }
}