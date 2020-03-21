using Scaglia.Entity.Model;
using System;
using System.Collections.Generic;

namespace Scaglia.Entity.Repository
{
    public sealed class GetByAttributesCallback
    {
        public Action<Int32, Int32, IList<IAttribute>, IList<IEntity>> Found { get; set; }
        public Action<Int32, Int32, IList<IAttribute>> NotFound { get; set; }
        public Action<Int32, Int32, IList<IAttribute>, Exception> Exception { get; set; }
    }
}
