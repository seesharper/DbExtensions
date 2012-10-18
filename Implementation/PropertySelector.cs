namespace DbExtensions.Implementation
{
    using System;
    using System.Linq;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A <see cref="IPropertySelector"/> implementation that returns properties with "simple" types.
    /// </summary>
    public class PropertySelector : IPropertySelector
    {
        private static readonly Type[] AdditionalTypes = new[]
            {
                typeof(string), 
                typeof(Guid), 
                typeof(decimal), 
                typeof(DateTime), 
                typeof(Enum),
                typeof(byte[]),
                typeof(char[])
            };
        
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return type.GetProperties().Where(t => TypeExtensions.GetUnderlyingType(t.PropertyType).IsSimpleType()).OrderBy(p => p.Name).ToArray();
        }        
    }
}