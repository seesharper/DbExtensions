namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Linq;

    using DbExtensions.Interfaces;

    public class OrdinalSelector : IOrdinalSelector
    {
        private readonly IPropertyMapper propertyMapper;

        public OrdinalSelector(IPropertyMapper propertyMapper)
        {
            this.propertyMapper = propertyMapper;
        }

        public int[] Execute(Type type, IDataRecord dataRecord)
        {
            return propertyMapper.Execute(type, dataRecord).Select(pm => pm.Ordinal).ToArray();
        }
    }
}