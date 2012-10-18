namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Linq;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that capable of returning the 
    /// ordinals used to map a type to a data record.
    /// </summary>
    public class OrdinalSelector : IOrdinalSelector
    {
        private readonly IPropertyMapper propertyMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdinalSelector"/> class.
        /// </summary>
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> that is responsible for mapping fields/columns from an <see cref="IDataRecord"/> to
        /// the properties of a <see cref="Type"/>.</param>
        public OrdinalSelector(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
        }

        /// <summary>
        /// Executes the selector and returns the ordinals required to read the columns from the data record.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return the ordinals.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A list of ordinals.</returns>
        public int[] Execute(Type type, IDataRecord dataRecord)
        {
            return propertyMapper.Execute(type, dataRecord).Select(pm => pm.Ordinal).ToArray();
        }
    }
}