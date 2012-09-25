namespace DbExtensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class ComplexPropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(t => !t.PropertyType.GetUnderlyingType().IsSimpleType()).ToArray();
        }
    }
}