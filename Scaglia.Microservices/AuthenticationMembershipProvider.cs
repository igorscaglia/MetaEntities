using System;
using Scaglia.Entity.Model;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Scaglia.Microservices
{
    public sealed class AuthenticationMembershipProvider : MembershipProvider
    {
        private const string AUTHENTICATEURL = "authenticateUrl";
        private const string GETTOKENDATAURL = "getTokenDataUrl";

        public AuthenticationMembershipProvider()
        {
            // Pegamos a instancia pelo dependencyresolver pois o asp.net não deixa injetar fácil como os controllers
            this.HttpRequester = (IHttpRequester)DependencyResolver.Current.GetService(typeof(HttpRequester));

            if (this.HttpRequester == null)
            {
                throw new NotImplementedException("DependencyResolver can't resolve HttpRequester.");
            }
        }

        public IHttpRequester HttpRequester { get; private set; }

        public string AuthenticateUrl { get; private set; }

        public string GetTokenDataUrl { get; private set; }

        public int IdSistema { get; private set; }

        public int IdOrganizacao { get; private set; }

        public ISession Sessao { get; private set; }

        public override bool EnablePasswordRetrieval { get; }

        public override bool EnablePasswordReset { get; }

        public override bool RequiresQuestionAndAnswer { get; }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts { get; }

        public override int PasswordAttemptWindow { get; }

        public override bool RequiresUniqueEmail { get; }

        public override MembershipPasswordFormat PasswordFormat { get; }

        public override int MinRequiredPasswordLength { get; }

        public override int MinRequiredNonAlphanumericCharacters { get; }

        public override string PasswordStrengthRegularExpression { get; }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentException("config");
            }

            if (String.IsNullOrEmpty(config[AUTHENTICATEURL]))
            {
                throw new ArgumentException(AUTHENTICATEURL);
            }
            else
            {
                this.AuthenticateUrl = config[AUTHENTICATEURL];
            }

            if (String.IsNullOrEmpty(config[GETTOKENDATAURL]))
            {
                throw new ArgumentException(GETTOKENDATAURL);
            }
            else
            {
                this.GetTokenDataUrl = config[GETTOKENDATAURL];
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MembershipProvider que usa o serviço de autenticação do web service Scaglia.Microservices.Authentication");
            }

            base.Initialize(name, config);

            this.ApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            bool validado = false;

            // Vamos recuperar a token
            string tokenKey = this.RecuperarTokenKey(username, password);

            if (!String.IsNullOrEmpty(tokenKey))
            {
                // Vamos recuperar dos dados da token
                IEntity dadosToken = this.RecuperarDadosToken(tokenKey);

                if (dadosToken != null)
                {
                    // Vamos criar uma nova sessão somando a token key e os dados da token
                    // e colocar o objeto na propriedade para que o chamador possa pegá-lo
                    this.Sessao = new Session()
                    {
                        IdSystem = dadosToken.GetAttributeValue<int>(Constantes.IDSYSTEM),
                        IdOrganization = dadosToken.GetAttributeValue<int>(Constantes.IDORGANIZATION),
                        IdGroup = dadosToken.GetAttributeValue<int>(Constantes.IDGROUP),
                        IsAdmin = dadosToken.GetAttributeValue<Boolean>(Constantes.ISADMIN),
                        Token = tokenKey
                    };

                    validado = true;
                }
            }

            return validado;
        }

        private string RecuperarTokenKey(string username, string password)
        {
            // Temos que chamar abaixo assim pois senão dá deadlock
            // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?redirectedfrom=MSDN&view=netframework-4.8#System_Threading_Tasks_Task_Run_System_Action_
            // Segredo é usar TaskEx com o Wait
            var task = TaskEx.Run<String>(async () =>
            {
                string jsonAuthForm = "{\"Email\":\"" + username + "\"," +
                            "\"Password\":\"" + password + "\"}";

                return await this.HttpRequester.PostRequest<String>(this.AuthenticateUrl, jsonAuthForm);
            });

            task.Wait();

            return task.Result;
        }

        private IEntity RecuperarDadosToken(string tokenKey)
        {
            // Temos que chamar abaixo assim pois senão dá deadlock
            // https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?redirectedfrom=MSDN&view=netframework-4.8#System_Threading_Tasks_Task_Run_System_Action_
            var task = TaskEx.Run<IEntity>(async () =>
            {
                String requestUriString = String.Format("{0}?tokenKey={1}", this.GetTokenDataUrl, tokenKey);
                return await this.HttpRequester.GetRequest<Entity.Model.Entity > (requestUriString);
            });

            task.Wait();

            return task.Result;
        }
    }
}
