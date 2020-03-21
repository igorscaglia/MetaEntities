using Scaglia.Entity.Model;
using Scaglia.Entity.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scaglia.Entity.Controller
{
    public interface IEntityController
    {
        IEntityRepository EntityRepository { get; set; }
        Task<IEntity> GetById(Int32 idSystem, Int32 idOrganization, Int32 id);
        Task<IEntity> GetByCode(Int32 idSystem, Int32 idOrganization, String code);
        Task<IList<IEntity>> GetByAttributes(Int32 idSystem, Int32 idOrganization, IList<IAttribute> attributes);
        Task<IEntity> Create(Int32 idSystem, Int32 idOrganization, IEntity entity);
        Task<IEntity> Update(Int32 idSystem, Int32 idOrganization, IEntity entity);
        Task<Boolean> Delete(Int32 idSystem, Int32 idOrganization, IEntity entity);
    }
}
