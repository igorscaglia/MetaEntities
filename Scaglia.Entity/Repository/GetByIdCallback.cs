using Scaglia.Entity.Model;
using System;

namespace Scaglia.Entity.Repository
{
    public sealed class GetByIdCallback
    {
        public Action<Int32, Int32, Int32, IEntity> Found { get; set; }
        public Action<Int32, Int32, Int32> NotFound { get; set; }
        public Action<Int32, Int32, Int32, Exception> Exception { get; set; }
    }
}
