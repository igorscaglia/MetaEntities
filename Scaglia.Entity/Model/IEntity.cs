using System;
using System.Collections.Generic;

namespace Scaglia.Entity.Model
{
    public interface IEntity : IEquatable<IEntity>
    {
        Int32 Id { get; set; }
        Int32? IdOrganization { get; set; }
        Int32? IdSystem { get; set; }
        String Name { get; set; }
        String Code { get; set; }
        IList<IEntityAttribute> EntityAttributes { get; set; }
        T GetAttributeValue<T>(string attributeName);
    }
}
