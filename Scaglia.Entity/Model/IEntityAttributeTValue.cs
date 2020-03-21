
namespace Scaglia.Entity.Model
{
    public interface IEntityAttributeValue<TValue> : IEntityAttributeValue
    {
        new TValue Value { get; set; }
    }
}
