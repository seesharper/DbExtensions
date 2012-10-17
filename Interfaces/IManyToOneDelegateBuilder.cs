namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a delegate that maps many to one relations.
    /// </summary>
    public interface IManyToOneDelegateBuilder<T>
    {
        /// <summary>
        /// Creates a delegate that maps many to one relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate used to map many to one relations.</returns>
        Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord);
    }
}