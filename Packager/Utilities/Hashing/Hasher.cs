using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Packager.Models.FileModels;

namespace Packager.Utilities.Hashing
{
    public class Hasher : IHasher
    {
        public Hasher(string baseProcessingFolder)
        {
            BaseProcessingFolder = baseProcessingFolder;
        }

        private string BaseProcessingFolder { get; }

        public async Task<string> Hash(AbstractFile model, CancellationToken cancellationToken)
        {
            return await Task.Run(() => HashInternal(Path.Combine(BaseProcessingFolder, model.GetFolderName(), model.Filename)), cancellationToken);
        }

        public async Task<string> Hash(string path,CancellationToken cancellationToken)
        {
            return await Task.Run(() => HashInternal(path), cancellationToken);
        }

        public static string Hash(Stream content)
        {
            using (var md5 = MD5.Create())
            {
                using (content)
                {
                    var hash = md5.ComputeHash(content);
                    return hash.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
                }
            }
        }
        
        private static string HashInternal(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Hash(stream);
            }
        }
    }
}