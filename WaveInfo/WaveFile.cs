using System.Collections.Generic;
using System.Linq;

namespace WaveInfo
{
    public class WaveFile
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public List<AbstractChunk> Chunks { get; set; }

        public T GetChunk<T>() where T : AbstractChunk
        {
            if (Chunks == null || !Chunks.Any())
            {
                return null;
            }

            return Chunks.FirstOrDefault(c => c.GetType() == typeof (T)) as T;
        }
    }
}