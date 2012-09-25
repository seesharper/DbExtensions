namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public interface IOrdinalSelector
    {
        IEnumerable<PropertyMappingInfo> Execute(Type type, IDictionary<string,int> columnOrdinals);
    }

    public class OrdinalSelector : IOrdinalSelector
    {
        private readonly IPropertySelector propertySelector;

        public OrdinalSelector(IPropertySelector propertySelector)
        {
            this.propertySelector = propertySelector;
        }

        public IEnumerable<PropertyMappingInfo> Execute(Type type, IDictionary<string, int> columnOrdinals)
        {            
            PropertyInfo[] properties = propertySelector.Execute(type);            
            for (int index = 0; index < properties.Length; index++)
            {                
                PropertyInfo property = properties[index];
                var propertyMappingInfo = new PropertyMappingInfo { Property = property };
                int ordinal;
                if (!columnOrdinals.TryGetValue(property.Name, out ordinal))
                {                    
                    if(!columnOrdinals.TryGetValue(type.Name + "_" + property.Name, out ordinal))
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