namespace DbExtensions.Decorators
{
    using System;
    using System.Data;
    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that caches the delegate used to produce an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the delegate.</typeparam>
    public class CachedInstanceDelegateBuilder<T> : IInstanceDelegateBuilder<T> where T : class
    {
        private readonly IInstanceDelegateBuilder<T> instanceDelegateBuilder;

        private Func<IDataRecord, T> cachedDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInstanceDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="instanceDelegateBuilder">
        /// The <see cref="IInstanceDelegateBuilder{T}"/> being decorated.
        /// </param>
        public CachedInstanceDelegateBuilder(IInstanceDelegateBuilder<T> instanceDelegateBuilder)
        {
            this.instanceDelegateBuilder = instanceDelegateBuilder;
        }

        /// <summary>
        /// Creates a delegate that 
        /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate that represents creating an instance of <typeparamref name="T"/>.</returns>
        public Func<IDataRecord, T> CreateInstanceDelegate(IDataRecord dataRecord)
        {
            return cachedDelegate ?? (cachedDelegate = instanceDelegateBuilder.CreateInstanceDelegate(dataRecord));
        }
    }
}