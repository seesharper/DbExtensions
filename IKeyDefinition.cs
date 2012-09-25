namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IKeyDefinition<T> : IEnumerable<PropertyInfo>
    {
        /// <summary>
        /// Defines a unique key for the generic type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="key">A function used to specify the key property.</param>
        void Define(params Expression<Func<T, object>>[] key);
    }
}