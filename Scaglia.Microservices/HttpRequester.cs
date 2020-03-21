using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scaglia.Microservices
{
    public sealed class HttpRequester : IHttpRequester
    {
        public async Task<T> GetRequest<T>(string requestUriString)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUriString);

            httpRequest.ContentType = Constantes.JSONCONTENTTYPE;
            httpRequest.Method = Constantes.GET_METHOD;

            return await GetResponseWithJson<T>(httpRequest);
        }

        public async Task<T> PostRequest<T>(string requestUriString, string jsonPostData)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUriString);

            httpRequest.ContentType = Constantes.JSONCONTENTTYPE;
            httpRequest.Method = Constantes.POST_METHOD;

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonPostData);
            }

            return await GetResponseWithJson<T>(httpRequest);
        }

        private static async Task<T> GetResponseWithJson<T>(HttpWebRequest httpRequest)
        {
            using (HttpWebResponse httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
            {
                // Forma de deserializar abaixo é para objetos complexos
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    string jsonResultText = streamReader.ReadToEnd();

                    return await jsonResultText.DeserializeWithJson<T>();
                }
            }
        }

    }
}
