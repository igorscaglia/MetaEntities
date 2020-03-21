using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Scaglia.Entity.Infrastructure
{
    internal sealed class CollectionJsonConverter<T, Tt> : JsonConverter where T : Tt
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IList<Tt>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IList<Tt> items = serializer.Deserialize<List<T>>(reader).Cast<Tt>().ToList();
            return items;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
