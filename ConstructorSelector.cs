namespace DbExtensions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Selects the first constructor found within a given <see cref="Type"/>.
    /// </summary>
    public class ConstructorSelector : IConstructorSelector
    {
        /// <summary>
        /// Returns a <see cref="ConstructorInfo"/> instance found within the target <typeref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return a constructor.</param>
        /// <returns>A <see cref="ConstructorInfo"/> that represents a constructor from the given <paramref name="type"/>.</returns>
        public ConstructorInfo Execute(Type type)
        {
            return type.GetConstructors()[0];
        }
    }
}