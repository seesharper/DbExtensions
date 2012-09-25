namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection.Emit;

    /// <summary>
    /// A <see cref="IMethodSkeleton{T}"/> implementation based on a <see cref="DynamicMethod"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicMethodSkeleton<T> : IMethodSkeleton<T>
    {
        private readonly DynamicMethod m_dynamicMethod = new DynamicMethod(string.Empty, typeof(T), new[] { typeof(IDataRecord), typeof(int[]) });

        /// <summary>
        /// Gets the <see cref="ILGenerator"/> used to emit the method body.
        /// </summary>
        /// <returns>An <see cref="ILGenerator"/> instance.</returns>
        public ILGenerator GetILGenerator()
        {
            return m_dynamicMethod.GetILGenerator();
        }

        /// <summary>
        /// Create a delegate used to invoke the dynamic method.
        /// </summary>
        /// <returns>A function delegate.</returns>
        public Func<IDataRecord, int[], T> CreateDelegate()
        {
            return (Func<IDataRecord, int[], T>)m_dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, int[], T>));
        }
    }
}
