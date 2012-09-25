namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents the skeleton of an dynamic method. 
    /// </summary>
    public interface IMethodSkeleton
    {
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> used to emit the method body.
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> instance.</returns>
        ILGenerator GetILGenerator();

        /// <summary>
        /// Create a delegate used to invoke the dynamic method.
        /// </summary>
        /// <param name="delegateType">
        /// A delegate type whose signature matches that of the dynamic method.  
        /// </param>
        /// <returns>
        /// A delegate of the specified type, which can be used to execute the dynamic method.
        /// </returns>
        Delegate CreateDelegate(Type delegateType);
    }
}