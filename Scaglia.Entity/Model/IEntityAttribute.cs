using System;

namespace Scaglia.Entity.Model
{
    public interface IEntityAttribute : IEquatable<IEntityAttribute>
    {
        Int32 Id { get; set; }
        Int32? IdEntity { get; set; }
        String Name { get; set; }
        EntityAttributeType? Type { get; set; }
        IEntityAttributeValue EntityAttributeValue { get; set; }
    }
}
