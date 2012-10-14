namespace DbExtensions.Implementation
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbExtensions.Interfaces;

    public class CollectionPropertySelector : IPropertySelector
    {
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(p => TypeExtensions.IsCollectionType(p.PropertyType)).ToArray();
        }        
    }
}