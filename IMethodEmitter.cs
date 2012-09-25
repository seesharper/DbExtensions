namespace DbExtensions
{
    using System;
    using System.Data;    

    /// <summary>
    /// Represents a class that is capable of emitting a dynamic method that populates an instance of a given type from an 
    /// <see cref="IDataRecord"/> instance.        
    /// </summary>        
    public interface IMethodEmitter
    {        
        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>      
        /// <param name="type">The target type for which to create the dynamic method.s</param>
        /// <returns>An function delegate used to invoke the method.</returns>
        Delegate CreateMethod(Type type); 
    }
}