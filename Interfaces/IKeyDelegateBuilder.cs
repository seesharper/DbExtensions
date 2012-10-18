namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// Represents a class that creates a delegate that is capable of retrieving key values 
    /// from a <see cref="IDataRecord"/> based on a <see cref="Type"/>.
    /// </summary>
    public interface IKeyDelegateBuilder
    {
        /// <summary>
        /// Creates a function delegate that returns a <see cref="IStructuralEquatable"/> instance, 
        /// representing the key values for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object for which to create a key delegate.</param>        
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the target fields/columns.</param>
        /// <returns>A function delegate used to retrieve key values for the given <paramref name="type"/>.</returns>
        Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(Type type, IDataRecord dataRecord);
    }   
}