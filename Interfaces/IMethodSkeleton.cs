namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Reflection.Emit;

    /// <summary>
    /// Represents the skeleton of a dynamic method.
    /// </summary>
    /// <typeparam name="T">The type of object returned by the dynamic method.</typeparam>
    public interface IMethodSkeleton<out T>
    {
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> used to emit the method body.
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> instance.</returns>
        ILGenerator GetILGenerator();

        /// <summary>
        /// Create a delegate used to invoke the dynamic method.
        /// </summary>
        /// <returns>A function delegate.</returns>
        Func<IDataRecord, int[], T> CreateDelegate();
    }
}