namespace DbExtensions.Implementation
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbExtensions.Interfaces;

    public class ComplexPropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(t => !TypeExtensions.GetUnderlyingType(t.PropertyType).IsSimpleType() && !TypeExtensions.GetUnderlyingType(t.PropertyType).IsCollectionType()).ToArray();
        }
    }
}