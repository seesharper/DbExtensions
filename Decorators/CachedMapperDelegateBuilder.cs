namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that caches the delegate used to create an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam>
    public class CachedMapperDelegateBuilder<T> : IMapperDelegateBuilder<T> 
    {
        private readonly IMapperDelegateBuilder<T> mapperDelegateBuilder;

        private readonly ConcurrentDictionary<Type, Func<IDataRecord, int[], T>> cache 
            = new ConcurrentDictionary<Type, Func<IDataRecord, int[], T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMapperDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="mapperDelegateBuilder">The <see cref="IMapperDelegateBuilder{T}"/> instance that is responsible for creating the delegate.</param>
        public CachedMapperDelegateBuilder(IMapperDelegateBuilder<T> mapperDelegateBuilder)
        {
            this.mapperDelegateBuilder = mapperDelegateBuilder;
        }

        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>      
        /// <param name="type">The target type for which to create the dynamic method.s</param>
        /// <returns>An function delegate used to invoke the method.</returns>
        public Func<IDataRecord, int[], T> CreateMethod(Type type)
        {
            return cache.GetOrAdd(type, t => mapperDelegateBuilder.CreateMethod(t));
        }
    }
}