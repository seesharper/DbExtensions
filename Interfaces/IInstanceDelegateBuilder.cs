namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a delegate that 
    /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the delegate.</typeparam>
    public interface IInstanceDelegateBuilder<T> where T : class
    {
        /// <summary>
        /// Creates a delegate that 
        /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate that represents creating an instance of <typeparamref name="T"/>.</returns>
        Func<IDataRecord, T> CreateInstanceDelegate(IDataRecord dataRecord);
    }
}