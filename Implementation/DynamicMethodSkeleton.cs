namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Reflection.Emit;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A <see cref="IMethodSkeleton{T}"/> implementation based on a <see cref="DynamicMethod"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the generated delegate.</typeparam>
    public class DynamicMethodSkeleton<T> : IMethodSkeleton<T>
    {
        private readonly DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(T), new[] { typeof(IDataRecord), typeof(int[]) });
        
        /// <summary>
        /// Gets the <see cref="ILGenerator"/> used to emit the method body.
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> instance.</returns>
        public ILGenerator GetILGenerator()
        {
            return dynamicMethod.GetILGenerator();
        }

        /// <summary>
        /// Create a delegate used to invoke the dynamic method.
        /// </summary>
        /// <returns>A function delegate.</returns>
        public Func<IDataRecord, int[], T> CreateDelegate()
        {
            return (Func<IDataRecord, int[], T>)dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, int[], T>));
        }
    }
}
