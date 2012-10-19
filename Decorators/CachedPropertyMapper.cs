namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;

    using DbExtensions.Interfaces;
    using DbExtensions.Model;

    /// <summary>
    /// A decorator that caches a list of <see cref="PropertyMappingInfo"/> instances per <see cref="Type"/>.
    /// </summary>
    public class CachedPropertyMapper : IPropertyMapper
    {
        private readonly IPropertyMapper propertyMapper;

        private readonly ConcurrentDictionary<Type, IEnumerable<PropertyMappingInfo>> cache 
            = new ConcurrentDictionary<Type, IEnumerable<PropertyMappingInfo>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertyMapper"/> class.
        /// </summary>
        /// <param name="propertyMapper">
        /// The <see cref="IPropertyMapper"/> being decorated.
        /// </param>
        public CachedPropertyMapper(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
        }

        /// <summary>
        /// Returns a list of <see cref="PropertyMappingInfo"/> instances that 
        /// represents the mapping between the fields from the <paramref name="dataRecord"/> and 
        /// the <paramref name="type"/> properties.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the target properties.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the target fields/columns.</param>
        /// <returns>A list of <see cref="PropertyMappingInfo"/> instances where each instance represents a match 
        /// between a field/column name and a property name.</returns>
        public IEnumerable<PropertyMappingInfo> Execute(Type type, IDataRecord dataRecord)
        {
            return cache.GetOrAdd(type, t => propertyMapper.Execute(type, dataRecord));
        }
    }
}