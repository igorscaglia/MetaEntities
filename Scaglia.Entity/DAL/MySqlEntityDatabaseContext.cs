
using Scaglia.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Scaglia.Entity.DAL
{
    public partial class MySqlEntityDatabaseContext
    {
        /// <summary>
        /// Abre a conexão e inicia um transação com o nível de isolamento read comitted
        /// </summary>
        /// <returns>Um DbTransaction representando a transação iniciada</returns>
        public DbTransaction StartReadCommittedTransaction()
        {
            if (this.Connection.State == System.Data.ConnectionState.Closed)
            {
                this.Connection.Open();
            }

            return this.Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }
    }

    public partial class EntityTable : IEntity
    {
        /// <summary>
        /// Recupera a lista de atributos que essa entidade tem
        /// </summary>
        IList<IEntityAttribute> IEntity.EntityAttributes
        {
            get
            {
                var entityAttributeList = (from ea in this.EntityAttributeTable
                                           select (IEntityAttribute)ea).ToList();

                return entityAttributeList;
            }
            set
            {
                throw new NotImplementedException("EntityAttributes");
            }
        }

        public bool Equals(IEntity other)
        {
            // É igual quando
            // Id, IdSystem e IOrganization ou
            // Code, IdSystem e IOrganization
            var isEqual =
                (this.Id.Equals(other.Id) &&
                this.IdSystem.Equals(other.IdSystem) &&
                this.IdOrganization.Equals(other.IdOrganization)) ||
                (this.Code.Equals(other.Code) &&
                this.IdSystem.Equals(other.IdSystem) &&
                this.IdOrganization.Equals(other.IdOrganization));

            return isEqual;
        }

        public T GetAttributeValue<T>(string attributeName)
        {
            throw new NotImplementedException("GetAttributeValue");
        }
    }

    public partial class EntityAttributeTable : IEntityAttribute
    {

        EntityAttributeType? IEntityAttribute.Type
        {
            get { return (EntityAttributeType?)this.Type; }
            set { this.Type = Convert.ToInt16(value); }
        }

        /// <summary>
        /// Recupera o valor do atributo
        /// </summary>
        public IEntityAttributeValue EntityAttributeValue
        {
            get
            {
                var value = this.GetValueByEntityAttributeType()[(EntityAttributeType)this.Type].Invoke();
                return value;
            }
            set
            {
                throw new NotImplementedException("EntityAttributeValue");
            }
        }

        public bool Equals(IEntityAttribute other)
        {
            var isEqual =
                (this.Id.Equals(other.Id) &&
                this.IdEntity.Equals(other.IdEntity)) ||
                (this.Name.Equals(other.Name) &&
                this.IdEntity.Equals(other.IdEntity));

            return isEqual;
        }

        private Dictionary<EntityAttributeType, Func<IEntityAttributeValue>> GetValueByEntityAttributeType()
        {
            // Através do tipo desse atributo recuperamos o valor na tabela certa
            Dictionary<EntityAttributeType, Func<IEntityAttributeValue>> values =
                new Dictionary<EntityAttributeType, Func<IEntityAttributeValue>>();

            values.Add(EntityAttributeType.Blob, () =>
            {
                this.BlobEntityAttributeTable.Load();
                return this.BlobEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.Boolean, () =>
            {
                this.BooleanEntityAttributeTable.Load();
                return this.BooleanEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.DateTime, () =>
            {
                this.DateTimeEntityAttributeTable.Load();
                return this.DateTimeEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.Double, () =>
            {
                this.DoubleEntityAttributeTable.Load();
                return this.DoubleEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.Float, () =>
            {
                this.FloatEntityAttributeTable.Load();
                return this.FloatEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.Integer, () =>
            {
                this.IntegerEntityAttributeTable.Load();
                return this.IntegerEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.MediumString, () =>
            {
                this.MediumStringEntityAttributeTable.Load();
                return this.MediumStringEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.String, () =>
            {
                this.StringEntityAttributeTable.Load();
                return this.StringEntityAttributeTable.FirstOrDefault();
            });
            values.Add(EntityAttributeType.TinyString, () =>
            {
                this.TinyStringEntityAttributeTable.Load();
                return this.TinyStringEntityAttributeTable.FirstOrDefault();
            });

            return values;
        }
    }

    public partial class BlobEntityAttributeTable : IEntityAttributeValue<Byte[]>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class BooleanEntityAttributeTable : IEntityAttributeValue<Boolean?>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class DateTimeEntityAttributeTable : IEntityAttributeValue<DateTime?>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class DoubleEntityAttributeTable : IEntityAttributeValue<Double?>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class FloatEntityAttributeTable : IEntityAttributeValue<Single?>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class IntegerEntityAttributeTable : IEntityAttributeValue<Int32?>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class MediumStringEntityAttributeTable : IEntityAttributeValue<String>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class StringEntityAttributeTable : IEntityAttributeValue<String>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }

    public partial class TinyStringEntityAttributeTable : IEntityAttributeValue<String>
    {
        dynamic IEntityAttributeValue.Value
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }
}
