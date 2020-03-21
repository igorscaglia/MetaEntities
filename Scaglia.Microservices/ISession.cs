
namespace Scaglia.Microservices
{
    public interface ISession
    {
        string Token { get; set; }
        int IdSystem { get; set; }
        int IdOrganization { get; set; }
        int IdGroup { get; set; }
        bool IsAdmin { get; set; }
    }
}
