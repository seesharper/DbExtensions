namespace DbExtensions.Implementation
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that returns complex properties.
    /// </summary>
    public class ComplexPropertySelector : IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return
                type.GetProperties().Where(
                    t => !t.PropertyType.GetUnderlyingType().IsSimpleType() && !t.PropertyType.GetUnderlyingType().IsCollectionType()).ToArray();
        }
    }
}