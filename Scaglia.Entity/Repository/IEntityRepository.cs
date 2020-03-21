using System;
using Scaglia.Entity.Model;
using System.Collections.Generic;

namespace Scaglia.Entity.Repository
{
    public interface IEntityRepository
    {
        void GetById(Int32 idSystem, Int32 idOrganization, Int32 id, GetByIdCallback getByIdCallback);
        void GetByCode(Int32 idSystem, Int32 idOrganization, String code, GetByCodeCallback getByCodeCallback);
        void GetByAttributes(Int32 idSystem, Int32 idOrganization, IList<IAttribute> attributes, GetByAttributesCallback getAllByAttributes);
        void Create(Int32 idSystem, Int32 idOrganization, IEntity entity, CreateCallback createCallback);
        void Update(Int32 idSystem, Int32 idOrganization, IEntity entity, UpdateCallback updateCallback);
        void Delete(Int32 idSystem, Int32 idOrganization, IEntity entity, DeleteCallback deleteCallback);
    }
}
