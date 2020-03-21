
namespace Scaglia.Entity.Model
{
    public class EntityAttributeValue : IEntityAttributeValue
    {
        public int Id { get; set; }
        public int? IdEntityAttribute { get; set; }
        public dynamic Value { get; set; }
    }
}
