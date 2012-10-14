namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that provides caching capabilities to an <see cref="IMethodSelector"/> instance. 
    /// </summary>
    public class CachedMethodSelector : IMethodSelector
    {
        private readonly IMethodSelector methodSelector;
        private readonly ConcurrentDictionary<Type, Lazy<MethodInfo>> cache = new ConcurrentDictionary<Type, Lazy<MethodInfo>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedMethodSelector"/> class.
        /// </summary>
        /// <param name="methodSelector">
        /// The <see cref="IMethodSelector"/> responsible for selecting a get method for a given <see cref="Type"/>.
        /// </param>
        public CachedMethodSelector(IMethodSelector methodSelector)
        {
            this.methodSelector = methodSelector;
        }

        /// <summary>
        /// Returns the appropriate get method for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to return the get method.</param>
        /// <returns>A get method used to pull a value from an <see cref="IDataRecord"/> instance.</returns>
        public MethodInfo Execute(Type type)
        {
            return cache.GetOrAdd(type, t => new Lazy<MethodInfo>(() => this.methodSelector.Execute(t))).Value;
        }
    }
}