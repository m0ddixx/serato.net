﻿using Serato.Net.Structs;
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
        public event FinishedParsingTracksEventHandler FinishedParsingTracks;
        private static Mutex mut;
        private FileStream stream = null;
        private BinaryReader reader = null;
        private List<AdatStruct> AdatList = null;

        private readonly IEnumerable<FieldPropertiesAttribute> _attributes =
            (IEnumerable<FieldPropertiesAttribute>)typeof(TrackInfo).GetCustomAttributes(
                typeof(FieldPropertiesAttribute), false);

        private readonly ILogger _logger = null;
        public SessionFileParser(bool bindEvents = true)
        {
            Initialize(FileWatcher.Instance, bindEvents);
        }

        public SessionFileParser(ILogger logger, FileWatcher fileWatcher, bool bindEvents = true)
        {
            _logger = logger;
            Initialize(fileWatcher, bindEvents);
        }

        private void Initialize(FileWatcher fileWatcher, bool bindEvents = true)
        {
            if (bindEvents)
                fileWatcher.SessionFileChanged += ParseSessionFile;
            AdatList = new List<AdatStruct>();
            mut = new Mutex();
        }

        private void ParseSessionFile(object sender, FileSystemEventArgs e)
        {
            mut.WaitOne();
            try
            {
                stream = File.OpenRead(e.FullPath);
                reader = new BinaryReader(stream);
                ParseFile();
                var tracks = ProcessTrackInfo();
                OnFinishedParsingTracks(new FinishedParsingTracksEventArgs(tracks));
            }
            catch (IOException exception)
            {
                _logger.Log(LogLevel.Error, exception.ToString());
                Console.WriteLine(exception);
            }
            finally
            {
                AdatList.Clear();
                reader?.Close();
                stream?.Close();
                mut.ReleaseMutex();
            }
        }

        private void ParseFile()
        {
            while (reader.BaseStream.Length - reader.BaseStream.Position >= 8)
            {
                ParseHeaderWithChunk();
            }
        }

        private void ParseHeaderWithChunk()
        {
            var header = new ChunkHeader()
            {
                Identifier = Encoding.UTF8.GetString(reader.ReadBytes(4)),
                Length = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4))
            };
            ParseChunk(header);
        }

        private void ParseChunk(ChunkHeader header)
        {
            Action parserTask = header.Identifier switch
            {
                "vrsn" => () => ParseVrsnChunk(header.Length),
                "oent" => ParseHeaderWithChunk,
                "adat" => () => ParseAdatChunk(header.Length),
                _ => null,
            };
            parserTask?.Invoke();
        }

        public Task ParseVrsnChunk(int length)
        {
            return Task.CompletedTask;
        }

        public Task ParseAdatChunk(int length)
        {
            AdatList.Add(new AdatStruct()
            {
                Data = reader.ReadBytes(length)
            });
            return Task.CompletedTask;
        }

        public IEnumerable<TrackInfo> ProcessTrackInfo()
        {
            foreach (var adat in AdatList)
            {
                var ti = new TrackInfo();
                using var mem = new MemoryStream(adat.Data);
                using var r = new BinaryReader(mem);
                while (r.PeekChar() > -1)
                {
                    var id = BinaryPrimitives.ReadInt32BigEndian(r.ReadBytes(4));
                    var length = BinaryPrimitives.ReadInt32BigEndian(r.ReadBytes(4));
                    var data = r.ReadBytes(length);
                    ParseField(id, data, ref ti);
                }
                _logger?.Log(LogLevel.Information, ti.ToString());
                Console.WriteLine(ti);
                yield return ti;
            }
        }

        private void ParseField(int id, byte[] data, ref TrackInfo ti)
        {
            var prop = typeof(TrackInfo).GetFields()
                .FirstOrDefault(p => (p.CustomAttributes?.Any(c =>
                        c.AttributeType == typeof(FieldPropertiesAttribute) &&
                        (int) c.ConstructorArguments[0].Value == id))
                    .GetValueOrDefault());
            if (prop == null) return;
            var t = prop.FieldType;
            if (t == typeof(int))
            {
                if (data.Length == 1)
                {
                    prop.SetValueDirect(__makeref(ti), (int)data[0]);
                }
                else
                {
                    prop.SetValueDirect(__makeref(ti), BinaryPrimitives.ReadInt32BigEndian(data));
                }
            }
            else if (t == typeof(string))
            {
                prop.SetValueDirect(__makeref(ti), Encoding.BigEndianUnicode.GetString(data).Replace("\0", string.Empty));
            }

        }

        private void OnFinishedParsingTracks(FinishedParsingTracksEventArgs e)
        {
            FinishedParsingTracks?.Invoke(this, e);
        }

        public delegate void FinishedParsingTracksEventHandler(object sender, FinishedParsingTracksEventArgs e);

        public void Dispose()
        {
            mut.Dispose();
            stream?.Dispose();
            reader?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await stream.DisposeAsync();
            mut.Dispose();
            reader.Dispose();
        }
    }
}