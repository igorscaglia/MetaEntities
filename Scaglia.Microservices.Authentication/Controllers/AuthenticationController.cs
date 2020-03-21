
using System;
using System.Web.Http;
using System.Threading.Tasks;
using System.Configuration;
using Scaglia.Entity.Controller;
using System.Linq;
using System.Collections.Generic;
using Scaglia.Entity.Model;
using Scaglia.Entity.Infrastructure;
using Scaglia.Entity.Repository;

namespace Scaglia.Microservices.Authentication.Controllers
{
    public class AuthenticationController : ApiController
    {
        private IEntityController _EntityController;

        public AuthenticationController(IEntityController entityController)
        {
            this._EntityController = entityController;
        }

        [HttpPost]
        public async Task<String> Authenticate([FromBody]AuthenticateForm authenticateForm)
        {
            int idSystem = Convert.ToInt32(ConfigurationManager.AppSettings["IdSystem"]);
            int idOrganization = Convert.ToInt32(ConfigurationManager.AppSettings["IdOrganization"]);

            // Cria um hash do password
            string hashedPass = SHA.GenerateSHA512String(authenticateForm.Password);

            IList<IAttribute> attributes = new List<IAttribute>();
            attributes.Add(new Entity.Model.Attribute() { Name = "Email", Value1 = authenticateForm.Email, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });
            attributes.Add(new Entity.Model.Attribute() { Name = "Password", Value1 = hashedPass, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });

            // Recupera a credencial comparando o email e seu password
            IList<IEntity> credentials = await this._EntityController.GetByAttributes(idSystem, idOrganization, attributes);

            // Se existir a credencial
            if (credentials != null && credentials.Count > 0)
            {
                var credential = credentials[0];
                // Verificar se já tem uma credencial, caso contrário cria

                IList<IAttribute> attrToken = new List<IAttribute>();
                attrToken.Add(new Entity.Model.Attribute() { Name = "IdCredential", Value1 = credential.Id, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });
                attrToken.Add(new Entity.Model.Attribute() { Name = "Enabled", Value1 = true, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });

                // Recupera a token pelo id, mas somente se estiver habilitada
                IList<IEntity> tokens = await this._EntityController.GetByAttributes(idSystem, idOrganization, attrToken);

                // Se existir a token retorna ela
                if (tokens != null && tokens.Count > 0)
                {
                    IEntity token = tokens[0];
                    var tokenKey = token.EntityAttributes.Where(x => x.Name == "Key").FirstOrDefault()
                        .EntityAttributeValue.Value;

                    return tokenKey;
                }
                else
                {

                    // Não existe uma key para essa credencial ou ela está desabilitada, vamos criar uma nova token
                    IEntity newToken = new Entity.Model.Entity()
                        .AddIdSystem(idSystem)
                        .AddIdOrganization(idOrganization)
                        .AddCode(RNGKey.New())
                        .AddName("Token")
                        .AddAttribute("IdCredential", EntityAttributeType.Integer, credential.Id)
                        .AddAttribute("Key", EntityAttributeType.TinyString, RNGKey.New())
                        .AddAttribute("Enabled", EntityAttributeType.Boolean, true)
                        .AddAttribute("CreatedDate", EntityAttributeType.DateTime, DateTime.Now);

                    // Cria a nova token e retorna ela atualizada com o seu id
                    IEntity token = await this._EntityController.Create(idSystem, idOrganization, newToken);

                    // Se criou a token retorna a key
                    if (token != null)
                    {
                        var tokenKey = token.EntityAttributes.Where(x => x.Name == "Key").FirstOrDefault().EntityAttributeValue.Value;
                        return tokenKey;
                    }
                }
            }

            return null;
        }

        [HttpGet]
        public async Task<Boolean> ValidateToken(string tokenKey)
        {
            int idSystem = Convert.ToInt32(ConfigurationManager.AppSettings["IdSystem"]);
            int idOrganization = Convert.ToInt32(ConfigurationManager.AppSettings["IdOrganization"]);

            IList<IAttribute> attributes = new List<IAttribute>();
            attributes.Add(new Entity.Model.Attribute() { Name = "Key", Value1 = tokenKey, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });
            attributes.Add(new Entity.Model.Attribute() { Name = "Enabled", Value1 = true, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });

            // Recupera a credencial comparando o email e seu password
            IList<IEntity> tokens = await this._EntityController.GetByAttributes(idSystem, idOrganization, attributes);

            // Se não nulo e houver pelo menos 1 token na lista então retorna true
            return tokens != null && tokens.Count > 0;
        }

        [HttpGet]
        public async Task<IEntity> GetTokenData(string tokenKey)
        {
            int idSystem = Convert.ToInt32(ConfigurationManager.AppSettings["IdSystem"]);
            int idOrganization = Convert.ToInt32(ConfigurationManager.AppSettings["IdOrganization"]);

            IList<IAttribute> attributes = new List<IAttribute>();
            attributes.Add(new Entity.Model.Attribute() { Name = "Key", Value1 = tokenKey, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });
            attributes.Add(new Entity.Model.Attribute() { Name = "Enabled", Value1 = true, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });

            // Recupera a credencial comparando o email e seu password
            IList<IEntity> tokens = await this._EntityController.GetByAttributes(idSystem, idOrganization, attributes);

            // Se existir a token
            if (tokens != null && tokens.Count > 0)
            {
                // Pega a primeira token
                var t = tokens[0];

                // O valor do campo IdCredential da Token
                int idCredential = (from a in t.EntityAttributes
                                    where
                                        a.Name.Equals("IdCredential")
                                    select a).SingleOrDefault().EntityAttributeValue.Value;

                // Recupera a credencial pelo seu id que está na token
                IEntity credential = await this._EntityController.GetById(idSystem, idOrganization, idCredential);

                // Recupera o valor da propriedade IsAdmin
                var isAdmin = (from a in credential.EntityAttributes
                               where
                                   a.Name.Equals("IsAdmin")
                               select a).SingleOrDefault().EntityAttributeValue.Value;

                // Recupera o valor da propriedade IdGroup
                var group = (from a in credential.EntityAttributes
                             where
                                 a.Name.Equals("IdGroup")
                             select a).SingleOrDefault().EntityAttributeValue.Value;

                // Vamos criar o retorno TokenData
                IEntity newTokenData = new Entity.Model.Entity()
                    .AddIdSystem(idSystem)
                    .AddIdOrganization(idOrganization)
                    .AddName("TokenData")
                    .AddAttribute("IdSystem", EntityAttributeType.Integer, idSystem)
                    .AddAttribute("IdOrganization", EntityAttributeType.Integer, idOrganization)
                    .AddAttribute("IdGroup", EntityAttributeType.Integer, group)
                    .AddAttribute("IsAdmin", EntityAttributeType.Boolean, isAdmin);

                return newTokenData;
            }

            return null;
        }

        [HttpGet]
        public async Task<Boolean> DisableToken(string tokenKey)
        {
            int idSystem = Convert.ToInt32(ConfigurationManager.AppSettings["IdSystem"]);
            int idOrganization = Convert.ToInt32(ConfigurationManager.AppSettings["IdOrganization"]);

            IList<IAttribute> attributes = new List<IAttribute>();
            attributes.Add(new Entity.Model.Attribute() { Name = "Key", Value1 = tokenKey, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });
            attributes.Add(new Entity.Model.Attribute() { Name = "Enabled", Value1 = true, Operator = Enum.GetName(typeof(AttributeOperator), AttributeOperator.Equals) });

            // Recupera a token
            IList<IEntity> tokens = await this._EntityController.GetByAttributes(idSystem, idOrganization, attributes);

            // Se existir a token
            if (tokens != null && tokens.Count > 0)
            {
                // Pega a primeira token
                var t = tokens[0];

                // Desabilita a token
                (from a in t.EntityAttributes
                 where
                     a.Name.Equals("Enabled")
                 select a).SingleOrDefault().EntityAttributeValue.Value = false;

                // Atualiza a token 
                IEntity updatedEntity = await this._EntityController.Update(idSystem, idOrganization, t);

                // Se retornou então atualizou a propriedade
                return updatedEntity != null;
            }

            return false;
        }

        [HttpGet]
        public string Version()
        {
            return Environment.Version.ToString();
        }

        public class AuthenticateForm
        {
            public String Email { get; set; }
            public String Password { get; set; }
        }
    }
}
