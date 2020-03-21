namespace Scaglia.Microservices
{
    public interface IAuthenticationProvider
    {
        bool Authenticate(string username, string password);
    }
}
