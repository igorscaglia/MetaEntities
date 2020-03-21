using Scaglia.Entity.Model;
using System;

namespace Scaglia.Entity.Repository
{
    public sealed class DeleteCallback
    {
        public Action<Int32, Int32, IEntity> Deleted { get; set; }
        public Action<Int32, Int32, IEntity> NotDeleted { get; set; }
        public Action<Int32, Int32, IEntity, Exception> Exception { get; set; }
    }
}
