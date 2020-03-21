
using System;
using System.Web.Http;
using System.Threading.Tasks;
using Scaglia.Entity.Controller;
using System.Linq;
using System.Collections.Generic;
using Scaglia.Entity.Model;

namespace Scaglia.Microservices.Entity.Controllers
{
    public class EntitiesController : BaseApiController
    {
        public EntitiesController(IEntityController entityController) : base(entityController) { }

        [HttpGet]
        public async Task<IEntity> GetById(String tokenKey, Int32 id)
        {
            return await this.Call<IEntity>(tokenKey, async () =>
            {
                return await this.EntityController.GetById(this.IdSystem, this.IdOrganization, id);
            });
        }

        [HttpGet]
        public async Task<IEntity> GetByCode(String tokenKey, String code)
        {
            return await this.Call<IEntity>(tokenKey, async () =>
            {
                return await this.EntityController.GetByCode(this.IdSystem, this.IdOrganization, code);
            });
        }

        [HttpPost]
        public async Task<IList<IEntity>> GetByAttributes(String tokenKey, IList<Scaglia.Entity.Model.Attribute> attributes)
        {
            return await this.Call<IList<IEntity>>(tokenKey, async () =>
            {
                return await this.EntityController.GetByAttributes(this.IdSystem, this.IdOrganization, attributes.Cast<IAttribute>().ToList());
            });
        }

        [HttpPost]
        public async Task<IEntity> Create(String tokenKey, [FromBody] Scaglia.Entity.Model.Entity entity)
        {
            return await this.Call<IEntity>(tokenKey, async () =>
            {
                return await this.EntityController.Create(this.IdSystem, this.IdOrganization, entity);
            });
        }

        [HttpPost]
        public async Task<IEntity> Update(String tokenKey, [FromBody] Scaglia.Entity.Model.Entity entity)
        {
            return await this.Call<IEntity>(tokenKey, async () =>
            {
                return await this.EntityController.Update(this.IdSystem, this.IdOrganization, entity);
            });
        }

        [HttpPost]
        public async Task<Boolean> Delete(String tokenKey, [FromBody] Scaglia.Entity.Model.Entity entity)
        {
            return await this.Call<Boolean>(tokenKey, async () =>
            {
                return await this.EntityController.Delete(this.IdSystem, this.IdOrganization, entity);
            });
        }

        [HttpGet]
        public string Version()
        {
            return Environment.Version.ToString();
        }
    }
}
