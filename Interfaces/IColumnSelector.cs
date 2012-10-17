namespace DbExtensions.Interfaces
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Represents a class that returns a dictionary containing the column name and the column ordinal.
    /// </summary>
    public interface IColumnSelector
    {
        /// <summary>
        /// Executes the selector and returns a dictionary containing the column names and their ordinals.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A dictionary containing the column names and their ordinals.</returns>
        IDictionary<string, int> Execute(IDataRecord dataRecord); 
    }
}