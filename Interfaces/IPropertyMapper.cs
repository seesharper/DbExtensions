namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;    
    using DbExtensions.Model;

    /// <summary>
    /// Represents a class that maps the fields from an <see cref="IDataRecord"/> to
    /// the properties of a <see cref="Type"/>.
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// Returns a list of <see cref="PropertyMappingInfo"/> instances that 
        /// represents the mapping between the fields from the <paramref name="dataRecord"/> and 
        /// the <paramref name="type"/> properties.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> containing the target properties.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that contains the target fields/columns.</param>
        /// <returns>A list of <see cref="PropertyMappingInfo"/> instances where each instance represents a match 
        /// between a field/column name and a property name.</returns>
        IEnumerable<PropertyMappingInfo> Execute(Type type, IDataRecord dataRecord);
    }
}