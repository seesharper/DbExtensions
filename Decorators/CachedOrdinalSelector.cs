namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that caches the ordinals for a given <see cref="Type"/>.
    /// </summary>
    public class CachedOrdinalSelector : IOrdinalSelector
    {
        private readonly IOrdinalSelector ordinalSelector;

        private readonly ConcurrentDictionary<Type, int[]> cache = new ConcurrentDictionary<Type, int[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOrdinalSelector"/> class.
        /// </summary>
        /// <param name="ordinalSelector">
        /// The <see cref="IOrdinalSelector"/> being decorated.
        /// </param>
        public CachedOrdinalSelector(IOrdinalSelector ordinalSelector)
        {
            this.ordinalSelector = ordinalSelector;
        }

        /// <summary>
        /// Executes the selector and returns the ordinals required to read the columns from the data record.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return the ordinals.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A list of ordinals.</returns>
        public int[] Execute(Type type, IDataRecord dataRecord)
        {
            return cache.GetOrAdd(type, t => ordinalSelector.Execute(t, dataRecord));            
        }
    }
}