using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sample.Helpers
{
    public static class ResourceHelpers
    {
        public static byte[] GetBytes(string resource)
        {
            using (var resourceStream = GetResourceStream(resource))
            using (var memoryStream = new MemoryStream())
            {
                resourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> GetBytesAsync(string resource)
        {
            using (var resourceStream = GetResourceStream(resource))
            using (var memoryStream = new MemoryStream())
            {
                await resourceStream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        static Stream GetResourceStream(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(resource));
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            return resourceStream;
        }
    }
}