using Serato.Net.Structs;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serato.Net.Services
{
    public sealed class SessionFileParser : IDisposable, IAsyncDisposable
    {
        private static Mutex mut = new Mutex();
        private FileStream stream = null;
        private BinaryReader reader = null;
        private List<AdatStruct> AdatList = null;
        public SessionFileParser()
        {
            FileWatcher.Instance.SessionFileChanged += ParseSessionFile;
            AdatList = new List<AdatStruct>();
        }

        private void ParseSessionFile(object sender, FileSystemEventArgs e)
        {
            mut.WaitOne();
            try
            {
                stream = File.OpenRead(e.FullPath);
                reader = new BinaryReader(stream);
                ParseFile();
                reader.Close();
                stream.Close();                               
                var list = ProcessTrackInfo().ToList();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                AdatList.Clear();
                reader.Close();
                stream.Close();
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
                    ParseField(id, data);
                }

                yield return ti;
            }
        }

        private void ParseField(int id, byte[] data)
        {
            Console.WriteLine($"Field ID: {id} Data: {Encoding.UTF8.GetString(data)}");
        }

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