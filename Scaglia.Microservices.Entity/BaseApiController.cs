
using Scaglia.Entity.Controller;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace Scaglia.Microservices.Entity
{
    public abstract class BaseApiController : ApiController
    {
        protected IEntityController EntityController { get; set; }
        protected int IdSystem { get; set; }
        protected int IdOrganization { get; set; }
        private String AuthenticationServerAddress { get; set; }

        public BaseApiController(IEntityController entityController)
        {
            this.EntityController = entityController;
            this.IdSystem = Convert.ToInt32(ConfigurationManager.AppSettings["IdSystem"]);
            this.IdOrganization = Convert.ToInt32(ConfigurationManager.AppSettings["IdOrganization"]);
            this.AuthenticationServerAddress = ConfigurationManager.AppSettings["AuthenticationServerAddress"];
        }

        protected async Task<T> Call<T>(string tokenKey, Func<Task<T>> function)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                String.Format(this.AuthenticationServerAddress, tokenKey));
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";

            var response = await request.GetResponseAsync();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var texto = sr.ReadToEnd();
                bool isValid = Convert.ToBoolean(Deserialize<Object>(new JsonMediaTypeFormatter(), texto));

                if (isValid)
                {
                    return await function.Invoke();
                }

                return default(T);
            }
        }

        private string Serialize<T>(MediaTypeFormatter formatter, T value)
        {
            // Create a dummy HTTP Content.
            Stream stream = new MemoryStream();
            var content = new StreamContent(stream);
            /// Serialize the object.
            formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
            // Read the serialized string.
            stream.Position = 0;
            return content.ReadAsStringAsync().Result;
        }

        private T Deserialize<T>(MediaTypeFormatter formatter, string str) where T : class
        {
            // Write the serialized string to a memory stream.
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            // Deserialize to an object of type T
            return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
        }
    }
}
