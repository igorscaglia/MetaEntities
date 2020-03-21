using System;

namespace Scaglia.Entity.Model
{
    public interface IAttribute
    {
        String Name { get; set; }
        dynamic Value1 { get; set; }
        String Operator { get; set; }
        dynamic Value2 { get; set; }
        IAttribute SetOperator(AttributeOperator @operator);
    }
}
