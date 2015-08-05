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
                    return hash.Aggregate(string.Empty, (current, b) => current + string.Format((string) "{0:x2}", (object) b));
                }
            }
        }
    }
}