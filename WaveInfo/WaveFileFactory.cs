using System;
using System.Collections.Generic;
using System.IO;

namespace WaveInfo
{
    public static class WaveFileFactory
    {
        private static readonly Dictionary<string, Type> ChunkReaders = new Dictionary<string, Type>
        {
            {"riff", typeof (RiffChunk)},
            {"rf64", typeof (RiffChunk)},
            {"ds64", typeof (Ds64Chunk)},
            {"fmt ", typeof (FormatChunk)},
            {"fact", typeof (FactChunk)},
            {"data", typeof (DataChunk)},
            {"bext", typeof(BextChunk) },
            {"list", typeof (ListChunk)},
            {"iart", typeof (InfoChunk)},
            {"icms", typeof (InfoChunk)},
            {"icmt", typeof (InfoChunk)},
            {"icop", typeof (InfoChunk)},
            {"icrd", typeof (InfoChunk)},
            {"ieng", typeof (InfoChunk)},
            {"ignr", typeof (InfoChunk)},
            {"ikey", typeof (InfoChunk)},
            {"imed", typeof (InfoChunk)},
            {"inam", typeof (InfoChunk)},
            {"iprd", typeof (InfoChunk)},
            {"isbj", typeof (InfoChunk)},
            {"isft", typeof (InfoChunk)},
            {"isrc", typeof (InfoChunk)},
            {"isrf", typeof (InfoChunk)},
            {"itch", typeof (InfoChunk)},
            {"iarl", typeof (InfoChunk)}
        };

        public static WaveFile OpenWaveFile(string path)
        {
            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException("Could not open file", path);
            }

            var result = new WaveFile
            {
                FileName = Path.GetFileName(path),
                FileSize = new FileInfo(path).Length,
                Chunks = new List<AbstractChunk>()
            };

            using (
                var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                while (reader.BaseStream.Position < result.FileSize)
                {
                    var id = new string(reader.ReadChars(4)).ToLowerInvariant();
                    var chunk = GetChunk(id);
                    result.Chunks.Add(chunk);
                    
                    chunk.ReadChunk(reader);
                    HanderDataChunk(reader, result, chunk as DataChunk);
                }
            }
            return result;
        }

        private static AbstractChunk GetChunk(string id)
        {
            if (ChunkReaders.ContainsKey(id) == false)
            {
                return new OtherChunk
                {
                    Id = id
                };
            }

            var result = Activator.CreateInstance(ChunkReaders[id]) as AbstractChunk;
            if (result == null)
            {
                throw new InvalidCastException($"Could not cast chunk object to AbstractChunk");
            }
            result.Id = id;
            return result;
        }

        private static void HanderDataChunk(BinaryReader reader, WaveFile waveFile, DataChunk chunk)
        {
            if (chunk == null)
            {
                return;
            }

            var ds64Chunk = waveFile.GetChunk<Ds64Chunk>();
            var offset = (ds64Chunk != null)
                ? Convert.ToInt64(ds64Chunk.DataSize)
                : Convert.ToInt64(chunk.Size);
            reader.BaseStream.Seek(offset, SeekOrigin.Current);
        }
    }
}