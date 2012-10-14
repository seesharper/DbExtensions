namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface IColumnSelector
    {
        IDictionary<string, int> Execute(IDataRecord dataRecord); 
    }

    public class ColumnSelector : IColumnSelector
    {
        public IDictionary<string, int> Execute(IDataRecord dataRecord)
        {
            IDictionary<string, int> result = new Dictionary<string, int>(dataRecord.FieldCount, StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < dataRecord.FieldCount; i++)
            {
                result.Add(dataRecord.GetName(i), i);
            }
            return result;
        }
    }

}