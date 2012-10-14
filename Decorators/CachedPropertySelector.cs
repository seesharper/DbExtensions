namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that provides caching capabilities to an <see cref="IPropertySelector"/> instance. 
    /// </summary>
    public class CachedPropertySelector : IPropertySelector
    {
        private readonly ConcurrentDictionary<Type, Lazy<PropertyInfo[]>> cache 
            = new ConcurrentDictionary<Type, Lazy<PropertyInfo[]>>();

        private readonly IPropertySelector propertySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertySelector"/> class.
        /// </summary>
        /// <param name="propertySelector">
        /// The <see cref="IPropertySelector"/> responsible for selecting a set of properties from a given <see cref="Type"/>.
        /// </param>
        public CachedPropertySelector(IPropertySelector propertySelector)
        {
            this.propertySelector = propertySelector;
        }

        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return cache.GetOrAdd(type, t => new Lazy<PropertyInfo[]>(() => propertySelector.Execute(t))).Value;
        }
    }
}