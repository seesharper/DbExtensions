namespace DbExtensions.Decorators
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;
    
    /// <summary>
    /// A decorator that caches delegates used map relations.
    /// </summary>
    /// <typeparam name="T">The type of object who's relations will be assigned.</typeparam>
    public class CachedRelationDelegateBuilder<T> : IRelationDelegateBuilder<T>
    {
        private readonly IRelationDelegateBuilder<T> relationDelegateBuilder;

        private Action<IDataRecord, T> cachedDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRelationDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="relationDelegateBuilder">
        /// The <see cref="IRelationDelegateBuilder{T}"/> being decorated.
        /// </param>
        public CachedRelationDelegateBuilder(IRelationDelegateBuilder<T> relationDelegateBuilder)
        {
            this.relationDelegateBuilder = relationDelegateBuilder;
        }

        /// <summary>
        /// Creates a delegate that maps related objects to an instance of <typeparamref name="T"/>.
        /// </summary>        
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate used to map related objects to an instance of <typeparamref name="T"/>.</returns>
        public Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord)
        {
            return cachedDelegate ?? (cachedDelegate = relationDelegateBuilder.CreateDelegate(dataRecord));
        }
    }
}