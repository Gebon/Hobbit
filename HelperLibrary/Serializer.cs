using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Server
{
    public static class Serializer
    {
        public static byte[] Serialize<T>(T obj)
        {
            var ms = new MemoryStream();
            using (var writer = new JsonTextWriter(new StreamWriter(ms)))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return ms.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            var ms = new MemoryStream(data);
            using (var reader = new JsonTextReader(new StreamReader(ms)))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
