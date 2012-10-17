namespace DbExtensions.Decorators
{
    using System.Collections.Generic;
    using System.Data;
    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorators that caches the available columns for the current <see cref="IDataRecord"/>.
    /// </summary>
    public class CachedColumnSelector : IColumnSelector
    {
        private readonly IColumnSelector columnSelector;

        private IDictionary<string, int> cachedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedColumnSelector"/> class.
        /// </summary>
        /// <param name="columnSelector">The <see cref="IColumnSelector"/> being decorated.</param>
        public CachedColumnSelector(IColumnSelector columnSelector)
        {
            this.columnSelector = columnSelector;
        }

        /// <summary>
        /// Executes the selector and returns a dictionary containing the column names and their ordinals.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A dictionary containing the column names and their ordinals.</returns>
        public IDictionary<string, int> Execute(IDataRecord dataRecord)
        {
            if (cachedValue == null)
            {
                cachedValue = columnSelector.Execute(dataRecord);
            }

            return cachedValue;
        }
    }
}