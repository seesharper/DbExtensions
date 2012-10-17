namespace DbExtensions.Interfaces
{
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of transforming an <see cref="IDataRecord"/> into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the mapper.</typeparam>
    public interface IDataRecordMapper<out T>
        where T : class
    {
        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <returns>An instance of <typeparamref name="T"/> that represents the result of the mapping/transformation.</returns>
        T Execute(IDataRecord dataRecord);
    }
}