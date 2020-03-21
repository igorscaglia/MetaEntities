using Newtonsoft.Json;
using Scaglia.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scaglia.Entity.Model
{
    public class EntityAttribute : IEntityAttribute
    {
        public int Id { get; set; }
        public int? IdEntity { get; set; }
        public string Name { get; set; }
        public EntityAttributeType? Type { get; set; }

        [JsonConverter(typeof(EntityJsonConverter<EntityAttributeValue, IEntityAttributeValue>))]
        public IEntityAttributeValue EntityAttributeValue { get; set; }

        public bool Equals(IEntityAttribute other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}
