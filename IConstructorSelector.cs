namespace DbExtensions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of selecting a <see cref="ConstructorInfo"/> from a given <see cref="Type"/>.
    /// </summary>
    public interface IConstructorSelector
    {
        /// <summary>
        /// Returns a <see cref="ConstructorInfo"/> instance found within the target <typeref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return a constructor.</param>
        /// <returns>A <see cref="ConstructorInfo"/> that represents a constructor from the given <paramref name="type"/>.</returns>
        ConstructorInfo Execute(Type type);
    }
}