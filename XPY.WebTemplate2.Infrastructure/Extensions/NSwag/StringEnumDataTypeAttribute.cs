using System;

namespace XPY.WebTemplate2.Infrastructure.Extensions.NSwag
{
    public class StringEnumDataTypeAttribute : Attribute
    {
        public Type EnumType { get; set; }
        public StringEnumDataTypeAttribute(Type type)
        {
            this.EnumType = type;
        }
    }
}