namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public static class DataRecordExtensions
    {
        public static IDictionary<string,int> GetAllNames(this IDataRecord record)
        {
            IDictionary<string, int> result = new Dictionary<string, int>(record.FieldCount, StringComparer.InvariantCultureIgnoreCase);
            
            for (int i = 0; i < record.FieldCount; i++)
            {
                result.Add(record.GetName(i), i);                
            }
            return result;
        }
    }
}