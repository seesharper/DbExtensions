namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Type[] AdditionalTypes = new[]
            {
                typeof(string), 
                typeof(Guid), 
                typeof(decimal), 
                typeof(DateTime), 
                typeof(Enum),
                typeof(byte[]),
                typeof(char[])
            }; 
        
        /// <summary>
        /// Gets the underlying type from a <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>If the <paramref name="type"/> is a <see cref="Nullable{T}"/>, 
        /// the underlying type is returned, otherwise the <paramref name="type"/> is returned.</returns>
        public static Type GetUnderlyingType(this Type type)
        {
            Type underLyingType = Nullable.GetUnderlyingType(type);
            return underLyingType ?? type;
        }

        /// <summary>
        /// Determines if the a given type is a <see cref="Nullable{T}"/> type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> is a <see cref="Nullable{T}"/> type, otherwise <b>false</b></returns>       
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Determines if a given type is a "simple" type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> is a "simple" type, otherwise <b>false</b></returns>       
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || AdditionalTypes.Contains(type);
        }

        /// <summary>
        /// Determines if a given type implements the <see cref="ICollection{T}"/> interface.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns><b>true</b> if the <paramref name="type"/> implements <see cref="ICollection{T}"/>, otherwise <b>false</b></returns>       
        public static bool IsCollectionType(this Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
    }
}