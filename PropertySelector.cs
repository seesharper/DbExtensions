namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;

    public class PropertySelector : IPropertySelector
    {
        private readonly IEnumerable<IPropertySelector> propertySelectors;

        public PropertySelector(IEnumerable<IPropertySelector> propertySelectors)
        {
            this.propertySelectors = propertySelectors;
        }

        public PropertyInfo[] Execute(Type type)
        {
            return propertySelectors.SelectMany(p => p.Execute(type)).ToArray();
        }
    }
}