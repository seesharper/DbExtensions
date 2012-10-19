namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    /// <summary>
    /// Represents a class that is capable of creating a delegate that populates an instance of a given type from an 
    /// <see cref="IDataRecord"/> instance.            
    /// </summary>
    /// <typeparam name="T">The type of object to be created.</typeparam>    
    public interface IMapperDelegateBuilder<out T>
    {        
        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>      
        /// <param name="type">The target type for which to create the dynamic method.s</param>
        /// <returns>An function delegate used to invoke the method.</returns>
        Func<IDataRecord, int[], T> CreateMethod(Type type);         
    }
}