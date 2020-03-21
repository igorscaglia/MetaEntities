using System;
using Model = Scaglia.Entity.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Scaglia.Microservices
{
    public sealed class EntitiesRepository : IEntitiesRepository
    {
        public IHttpRequester HttpRequester { get; private set; }
        private string ServicesUrl { get; set; }

        public EntitiesRepository(IHttpRequester httpRequester)
        {
            if (httpRequester == null)
            {
                throw new ArgumentException("HttpRequester");
            }
            else
            {
                this.HttpRequester = httpRequester;
            }

            this.ServicesUrl = ConfigurationManager.AppSettings[Constantes.SCAGLIAMICROSERVICESENTITIESURL];

            // Se não foi configurado no app settings a url do serviço de entidades então dá erro
            if (String.IsNullOrEmpty(this.ServicesUrl))
            {
                throw new ArgumentException(Constantes.SCAGLIAMICROSERVICESENTITIESURL);
            }
        }

        public async Task<Model.IEntity> GetById(string tokenKey, int id)
        {
            String requestUriString = String.Format("{0}/GetById/{1}?tokenKey={2}",
                this.ServicesUrl, id, tokenKey);

            return await this.HttpRequester.GetRequest<Model.Entity>(requestUriString);
        }

        public async Task<Model.IEntity> GetByCode(string tokenKey, string code)
        {
            String requestUriString = String.Format("{0}/GetByCode?tokenKey={1}&code={2}",
                this.ServicesUrl, code, tokenKey);

            return await this.HttpRequester.GetRequest<Model.Entity>(requestUriString);
        }

        public async Task<IList<Model.Entity>> GetByAttributes(string tokenKey, IList<Model.IAttribute> attributes)
        {
            String requestUriString = String.Format("{0}/GetByAttributes?tokenKey={1}",
                this.ServicesUrl, tokenKey);

            String jsonAttributes = await attributes.SerializeWithJson<List<Model.IAttribute>>();

            return await this.HttpRequester.PostRequest<IList<Model.Entity>>(requestUriString, jsonAttributes);
        }

        public async Task<Model.IEntity> Create(string tokenKey, Model.IEntity entity)
        {
            String requestUriString = String.Format("{0}/Create?tokenKey={1}",
                this.ServicesUrl, tokenKey);

            String jsonAttributes = await entity.SerializeWithJson<Model.Entity>();

            return await this.HttpRequester.PostRequest<Model.Entity>(requestUriString, jsonAttributes);
        }

        public async Task<Model.IEntity> Update(string tokenKey, Model.IEntity entity)
        {
            String requestUriString = String.Format("{0}/Update?tokenKey={1}",
                this.ServicesUrl, tokenKey);

            String jsonAttributes = await entity.SerializeWithJson<Model.Entity>();

            return await this.HttpRequester.PostRequest<Model.Entity>(requestUriString, jsonAttributes);
        }

        public async Task<Boolean> Delete(string tokenKey, Model.IEntity entity)
        {
            String requestUriString = String.Format("{0}/Delete?tokenKey={1}",
                this.ServicesUrl, tokenKey);

            String jsonAttributes = await entity.SerializeWithJson<Model.Entity>();

            return await this.HttpRequester.PostRequest<Boolean>(requestUriString, jsonAttributes);
        }
    }
}
