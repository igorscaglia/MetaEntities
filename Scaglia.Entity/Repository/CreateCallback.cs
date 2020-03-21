using Scaglia.Entity.Model;
using System;

namespace Scaglia.Entity.Repository
{
    public sealed class CreateCallback
    {
        public Action<Int32, Int32, IEntity> Created { get; set; }
        public Action<Int32, Int32, IEntity> NotCreated { get; set; }
        public Action<Int32, Int32, IEntity, Exception> Exception { get; set; }
    }
}
