
namespace Scaglia.Microservices
{
    public sealed class Session: ISession
    {
        public string Token { get; set; }
        public int IdSystem { get; set; }
        public int IdOrganization { get; set; }
        public int IdGroup { get; set; }
        public bool IsAdmin { get; set; }
    }
}

