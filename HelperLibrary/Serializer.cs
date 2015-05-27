using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;

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

        public static T Deserialize<T>(Socket socket)
        {
            var ms = new NetworkStream(socket);
            using (var reader = new JsonTextReader(new StreamReader(ms)))
            {
                var serializer = new JsonSerializer();
                T result = default(T);
                try
                {
                    result = serializer.Deserialize<T>(reader);
                }
                catch (IOException)
                {
                    Environment.Exit(-1);
                }
                return result;
            }
        }
    }
}
