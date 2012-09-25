namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of resolving the ordinal 
    /// </summary>
    public interface IPropertyMapper
    {                
         PropertyMappingInfo Execute(IDataRecord dataRecord, PropertyInfo property);
    }

    public class PropertyMapper : IPropertyMapper
    {        
        public PropertyMappingInfo Execute(IDataRecord dataRecord, PropertyInfo property)
        {
            try
            {
                return new PropertyMappingInfo { Ordinal = dataRecord.GetOrdinal(property.Name), Property = property };
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException("property", "Unable to find a matching column name for property {0}".FormatWith(property));
            }                           
        }        
    }
}