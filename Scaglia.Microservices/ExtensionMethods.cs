using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Scaglia.Microservices
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Serializa o objeto em string
        /// </summary>
        /// <typeparam name="T">Tipo real desse objeto</typeparam>
        /// <param name="formatter">Instância de um MediaTypeFormatter</param>
        /// <returns></returns>
        public static async Task<string> Serialize<T>(this Object instance, MediaTypeFormatter formatter)
        {
            // Create a dummy HTTP Content.
            using (Stream stream = new MemoryStream())
            {
                using (var content = new StreamContent(stream))
                {
                    /// Serialize the object.
                    await formatter.WriteToStreamAsync(typeof(T), instance, stream, content, null);

                    // Read the serialized string.
                    stream.Position = 0;
                    return await content.ReadAsStringAsync();
                }
            }
        }

        public static async Task<string> SerializeWithJson<T>(this Object instance)
        {
            return await instance.Serialize<T>(new JsonMediaTypeFormatter());
        }

        public static async Task<T> Deserialize<T>(this string str, MediaTypeFormatter formatter)
        {
            // Write the serialized string to a memory stream.
            using (Stream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                    stream.Position = 0;
                    dynamic obj = await formatter.ReadFromStreamAsync(typeof(T), stream, null, null);

                    // Sem Cast
                    return obj;
                }
            }
        }

        public static async Task<T> DeserializeWithJson<T>(this string str)
        {
            return await str.Deserialize<T>(new JsonMediaTypeFormatter());
        }
    }
}
