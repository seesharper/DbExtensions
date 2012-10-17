namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a delegate that assigns related objects to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object who's relations will be assigned.</typeparam>
    public interface IRelationDelegateBuilder<in T>
    {
        /// <summary>
        /// Creates a delegate that maps related objects to an instance of <typeparamref name="T"/>.
        /// </summary>        
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate used to map related objects to an instance of <typeparamref name="T"/>.</returns>
        Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord);
    }
}