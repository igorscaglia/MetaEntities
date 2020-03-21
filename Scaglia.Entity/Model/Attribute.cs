using System;

namespace Scaglia.Entity.Model
{
    public class Attribute : IAttribute
    {
        public string Name { get; set; }
        public dynamic Value1 { get; set; }
        public String Operator { get; set; }
        public dynamic Value2 { get; set; }

        public IAttribute SetOperator(AttributeOperator @operator)
        {
            this.Operator = Enum.GetName(typeof(AttributeOperator), @operator);
            return this;
        }
    }
}
