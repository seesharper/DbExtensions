namespace DbExtensions.Decorators
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorators that caches the key delegate for a given type.
    /// </summary>
    public class CachedKeyDelegateBuilder : IKeyDelegateBuilder
    {
        private readonly IKeyDelegateBuilder keyDelegateBuilder;
        private readonly ConcurrentDictionary<Type, Func<IDataRecord, IStructuralEquatable>> cache 
            = new ConcurrentDictionary<Type, Func<IDataRecord, IStructuralEquatable>>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedKeyDelegateBuilder"/> class.
        /// </summary>
        /// <param name="keyDelegateBuilder">The <see cref="IKeyDelegateBuilder"/> instance that is 
        /// responsible for creating a key delegate for a given type.</param>
        public CachedKeyDelegateBuilder(IKeyDelegateBuilder keyDelegateBuilder)
        {
            this.keyDelegateBuilder = keyDelegateBuilder;
        }

        /// <summary>
        /// Creates a function delegate that returns a <see cref="IStructuralEquatable"/> instance, 
        /// representing the key values for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to build the delegate.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the target fields/columns.</param>
        /// <returns>A function delegate used to retrieve key values for the given <paramref name="type"/>.</returns>
        public Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(Type type, IDataRecord dataRecord)
        {
            return cache.GetOrAdd(type, t => keyDelegateBuilder.CreateKeyDelegate(t, dataRecord));
        }
    }
}