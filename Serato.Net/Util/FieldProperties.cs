using System;

namespace Serato.Net.Util
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FieldPropertiesAttribute : Attribute
    {
        public int FieldId;

        public FieldPropertiesAttribute(int id)
        {
            FieldId = id;
        }

    }
}