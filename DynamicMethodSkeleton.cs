namespace DbExtensions
{
    using System;
    using System.Reflection.Emit;

    /// <summary>
    /// A <see cref="IMethodSkeleton"/> implementation that wraps an <see cref="DynamicMethod"/>.
    /// </summary>
    public class DynamicMethodSkeleton : IMethodSkeleton
    {
        private readonly DynamicMethod dynamicMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMethodSkeleton"/> class.
        /// </summary>
        /// <param name="returnType">
        /// A <see cref="Type"/> object that specifies the return type of the dynamic method, or <b>null</b> if the method has no return type. 
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type"/> objects specifying the types of the parameters of the dynamic method, or <b>null</b> if the method has no parameters. 
        /// </param>
        public DynamicMethodSkeleton(Type returnType, Type[] parameterTypes)
        {
            dynamicMethod = new DynamicMethod(string.Empty, returnType, parameterTypes);
        }

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
        /// <param name="delegateType">
        /// A delegate type whose signature matches that of the dynamic method.  
        /// </param>
        /// <returns>
        /// A delegate of the specified type, which can be used to execute the dynamic method.
        /// </returns>
        public Delegate CreateDelegate(Type delegateType)
        {                         
            return dynamicMethod.CreateDelegate(delegateType);
        }
    }
}
