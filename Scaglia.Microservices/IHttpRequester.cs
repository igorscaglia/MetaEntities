using System.Threading.Tasks;

namespace Scaglia.Microservices
{
    public interface IHttpRequester
    {
        Task<T> GetRequest<T>(string requestUriString);
        Task<T> PostRequest<T>(string requestUriString, string jsonPostData);
    }
}
