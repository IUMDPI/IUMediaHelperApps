using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Packager.Utilities
{
    public class Hasher : IHasher
    {
        public string Hash(Stream content)
        {
            using (var md5 = MD5.Create())
            {
                using (content)
                {
                    var hash = md5.ComputeHash(content);
                    return hash.Aggregate(string.Empty, (current, b) => current + string.Format("{0:x2}", b));
                }
            }
        }

        public string Hash(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Hash(stream);
            }
        }
    }
}