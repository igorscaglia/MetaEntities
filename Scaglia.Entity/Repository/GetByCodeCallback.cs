using Scaglia.Entity.Model;
using System;

namespace Scaglia.Entity.Repository
{
    public sealed class GetByCodeCallback
    {
        public Action<Int32, Int32, String, IEntity> Found { get; set; }
        public Action<Int32, Int32, String> NotFound { get; set; }
        public Action<Int32, Int32, String, Exception> Exception { get; set; }
    }
}
