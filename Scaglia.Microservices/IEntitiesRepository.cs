using System.Collections.Generic;
using System.Threading.Tasks;
using Scaglia.Entity.Model;

namespace Scaglia.Microservices
{
    public interface IEntitiesRepository
    {
        Task<IEntity> Create(string tokenKey, IEntity entity);
        Task<bool> Delete(string tokenKey, IEntity entity);
        Task<IList<Entity.Model.Entity>> GetByAttributes(string tokenKey, IList<IAttribute> attributes);
        Task<IEntity> GetByCode(string tokenKey, string code);
        Task<IEntity> GetById(string tokenKey, int id);
        Task<IEntity> Update(string tokenKey, IEntity entity);
    }
}
