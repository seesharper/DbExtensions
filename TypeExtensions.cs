﻿namespace DbExtensions
{
    using System;
    using System.Reflection;
    using System.Linq;

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
        
        public static Type GetUnderlyingType(this Type type)
         {
             Type underLyingType = Nullable.GetUnderlyingType(type);
             return underLyingType ?? type;
         }

        public static ConstructorInfo GetNullableConstructor(this Type type)
        {
            return typeof(Nullable<>).MakeGenericType(type).GetConstructor(new Type[] { type });
        }

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || AdditionalTypes.Contains(type);
        }    
    }
}