namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A decorator that caches the <see cref="ConstructorInfo"/> for a given type.
    /// </summary>
    public class CachedConstructorSelector : IConstructorSelector
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> Cache = new ConcurrentDictionary<Type, ConstructorInfo>();
        
        private readonly IConstructorSelector constructorSelector;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedConstructorSelector"/> class.
        /// </summary>
        /// <param name="constructorSelector">
        /// The <see cref="IConstructorSelector"/> being decorated.
        /// </param>
        public CachedConstructorSelector(IConstructorSelector constructorSelector)
        {
            this.constructorSelector = constructorSelector;
        }

        /// <summary>
        /// Returns a <see cref="ConstructorInfo"/> instance found within the target <typeref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return a constructor.</param>
        /// <returns>A <see cref="ConstructorInfo"/> that represents a constructor from the given <paramref name="type"/>.</returns>
        public ConstructorInfo Execute(Type type)
        {
            return Cache.GetOrAdd(type, t => constructorSelector.Execute(type));
        }
    }
}