using Scaglia.Entity.Model;
using System;

namespace Scaglia.Entity.Repository
{
    public sealed class UpdateCallback
    {
        public Action<Int32, Int32, IEntity> Updated { get; set; }
        public Action<Int32, Int32, IEntity> NotUpdated { get; set; }
        public Action<Int32, Int32, IEntity, Exception> Exception { get; set; }
    }
}
