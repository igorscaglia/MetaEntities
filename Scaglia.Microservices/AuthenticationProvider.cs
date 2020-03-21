using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Scaglia.Microservices
{
    public sealed class AuthenticationProvider : IAuthenticationProvider
    {
        public bool Authenticate(string username, string password)
        {
            bool autenticado = Membership.ValidateUser(username, password);

            if (autenticado)
            {
                ISession sessaoAutenticada = ((AuthenticationMembershipProvider)Membership.Provider).Sessao;

                string jsonSessao = this.SerializarSessaoComoJson(sessaoAutenticada);

                // https://docs.microsoft.com/en-us/aspnet/web-forms/overview/older-versions-security/introduction/forms-authentication-configuration-and-advanced-topics-cs#step-4-storing-additional-user-data-in-the-ticket
                HttpCookie authCookie = FormsAuthentication.GetAuthCookie(username, false);
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(
                    ticket.Version,
                    ticket.Name,
                    ticket.IssueDate,
                    ticket.Expiration,
                    ticket.IsPersistent,
                    jsonSessao);
                authCookie.Value = FormsAuthentication.Encrypt(newTicket);
                HttpContext.Current.Response.Cookies.Add(authCookie);

                // Pegar a sessao pelo injetor
                ISession sessao = (ISession)DependencyResolver.Current.GetService(typeof(ISession));
                sessao.IdSystem = sessaoAutenticada.IdSystem;
                sessao.IdOrganization = sessaoAutenticada.IdOrganization;
                sessao.IdGroup = sessaoAutenticada.IdGroup;
                sessao.IsAdmin = sessaoAutenticada.IsAdmin;
                sessao.Token = sessaoAutenticada.Token;
                HttpContext.Current.Session[Constantes.SESSION] = sessao;

                // Antigo... aqui já autenticava mas não guardava os dados da sessao
                // FormsAuthentication.SetAuthCookie(username, false);
            }
            return autenticado;
        }

        private string SerializarSessaoComoJson(ISession sessao)
        {
            var task = TaskEx.Run<String>(async () =>
            {
                return await sessao.SerializeWithJson<Session>();
            });

            task.Wait();

            return task.Result;
        }
    }
}
