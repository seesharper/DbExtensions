namespace DbExtensions.Interfaces
{
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of transforming an <see cref="IDataReader"/> into an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the mapper.</typeparam>
    public interface IDataReaderMapper<out T> where T : class
    {
        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="dataReader">The target <see cref="IDataReader"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> that represents the result of the mapping/transformation.</returns>
        IEnumerable<T> Execute(IDataReader dataReader);
    }
}