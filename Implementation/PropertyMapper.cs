namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using DbExtensions.Interfaces;
    using DbExtensions.Model;

    /// <summary>
    /// A class that maps the fields from an <see cref="IDataRecord"/> to
    /// the properties of a <see cref="Type"/>.
    /// </summary>
    public class PropertyMapper : IPropertyMapper
    {
        private readonly IPropertySelector propertySelector;

        private readonly IColumnSelector columnSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapper"/> class.
        /// </summary>
        /// <param name="propertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting the 
        /// target properties for a given <see cref="Type"/>.</param>
        /// <param name="columnSelector">The <see cref="IColumnSelector"/> that is responsible for selecting column names 
        /// and column ordinals from a given <see cref="IDataRecord"/>.</param>
        public PropertyMapper(IPropertySelector propertySelector, IColumnSelector columnSelector)
        {
            this.propertySelector = propertySelector;
            this.columnSelector = columnSelector;
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
            var columnOrdinals = this.columnSelector.Execute(dataRecord);
            PropertyInfo[] properties = this.propertySelector.Execute(type);            
            for (int index = 0; index < properties.Length; index++)
            {                
                PropertyInfo property = properties[index];
                var propertyMappingInfo = new PropertyMappingInfo { Property = property };
                int ordinal;
                if (!columnOrdinals.TryGetValue(property.Name, out ordinal))
                {
                    if (!columnOrdinals.TryGetValue(type.Name + "_" + property.Name, out ordinal))
                    {
                        ordinal = -1;
                    }
                }

                propertyMappingInfo.Ordinal = ordinal;
                yield return propertyMappingInfo;
            }            
        }
    }
}