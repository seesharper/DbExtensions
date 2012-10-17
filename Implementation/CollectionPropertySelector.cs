namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DbExtensions.Interfaces;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that returns properties that implements the <see cref="ICollection{T}"/> interface.
    /// </summary>
    public class CollectionPropertySelector : IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(p => p.PropertyType.IsCollectionType()).ToArray();
        }        
    }
}