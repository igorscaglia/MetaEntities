using System;

namespace Scaglia.Entity.Model
{
    public interface IEntityAttributeValue
    {
        Int32 Id { get; set; }
        Int32? IdEntityAttribute { get; set; }
        dynamic Value { get; set; }
    }
}
