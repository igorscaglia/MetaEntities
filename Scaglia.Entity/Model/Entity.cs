using Newtonsoft.Json;
using Scaglia.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Scaglia.Entity.Model
{
    public class Entity : IEntity
    {
        public Entity()
        {
            this.EntityAttributes = new List<IEntityAttribute>();
        }

        public int Id { get; set; }
        public int? IdOrganization { get; set; }
        public int? IdSystem { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        [JsonConverter(typeof(CollectionJsonConverter<EntityAttribute, IEntityAttribute>))]
        public IList<IEntityAttribute> EntityAttributes { get; set; }

        public Entity AddId(int id)
        {
            this.Id = id;
            return this;
        }

        public Entity AddIdSystem(int idSystem)
        {
            this.IdSystem = idSystem;
            return this;
        }

        public Entity AddIdOrganization(int idOrganization)
        {
            this.IdOrganization = idOrganization;
            return this;
        }

        public Entity AddCode(string code)
        {
            this.Code = code;
            return this;
        }

        public Entity AddName(string name)
        {
            this.Name = name;
            return this;
        }

        public Entity AddAttribute(string name, EntityAttributeType type, dynamic value)
        {
            this.EntityAttributes.Add(new EntityAttribute()
            {
                Name = name,
                Type = type,
                EntityAttributeValue = new EntityAttributeValue()
                {
                    Value = value
                }
            });
            return this;
        }

        public T GetAttributeValue<T>(string attributeName)
        {
            var value = (from a in this.EntityAttributes
                         where
                             a.Name.Equals(attributeName)
                         select a).SingleOrDefault().EntityAttributeValue.Value;
            return (T)value;
        }

        public bool Equals(IEntity other)
        {
            return this.Id.Equals(other.Id);
        }
    }
}
