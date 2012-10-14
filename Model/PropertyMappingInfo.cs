namespace DbExtensions.Model
{
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents the mapping between a <see cref="PropertyInfo"/> and the ordinal in the target <see cref="IDataRecord"/>.
    /// </summary>
    public class PropertyMappingInfo
    {
        /// <summary>
        /// Gets or sets the <see cref="PropertyInfo"/> that has an ordinal in the target <see cref="IDataRecord"/>.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Gets or sets the ordinal for the <see cref="Property"/>.
        /// </summary>
        public int Ordinal { get; set; }
    }
}