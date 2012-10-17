namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that returns a dictionary containing the column name and the column ordinal.
    /// </summary>
    public class ColumnSelector : IColumnSelector
    {
        /// <summary>
        /// Executes the selector and returns a dictionary containing the column names and their ordinals.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A dictionary containing the column names and their ordinals.</returns>
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