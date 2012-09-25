namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of providing a <see cref="MethodInfo"/> that targets 
    /// a <see cref="IDataRecord"/> instance.    
    /// </summary>
    public interface IMethodSelector
    {        
        /// <summary>
        /// Returns the appropriate get method for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to return the get method.</param>
        /// <returns>A get method used to pull a value from an <see cref="IDataRecord"/> instance.</returns>
        MethodInfo Execute(Type type);
    }
}