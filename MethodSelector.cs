namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Provides the appropriate get method for a given <see cref="Type"/>.
    /// </summary>
    public class MethodSelector : IMethodSelector
    {
        private static readonly IDictionary<Type, Func<MethodInfo>> DefaultGetterMethods
            = new Dictionary<Type, Func<MethodInfo>>();
        
        static MethodSelector()
        {
            InitializeDefaultGetterMethods();
        }
       
        /// <summary>
        /// Returns the appropriate get method for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to return the get method.</param>
        /// <returns>A get method used to pull a value from an <see cref="IDataRecord"/> instance.</returns>
        public virtual MethodInfo Execute(Type type)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (DefaultGetterMethods.ContainsKey(underlyingType))
            {
                return DefaultGetterMethods[underlyingType]();
            }

            throw new InvalidOperationException(ErrorMessages.GetMethodNotFound.FormatWith(type));
        }

        private static void InitializeDefaultGetterMethods()
        {
            DefaultGetterMethods.Add(typeof(bool), () => typeof(IDataRecord).GetMethod("GetBoolean"));
            DefaultGetterMethods.Add(typeof(byte), () => typeof(IDataRecord).GetMethod("GetByte"));
            DefaultGetterMethods.Add(typeof(byte[]), () => typeof(StreamHelper).GetMethod("ReadByteArray"));
            DefaultGetterMethods.Add(typeof(char), () => typeof(IDataRecord).GetMethod("GetChar"));
            DefaultGetterMethods.Add(typeof(char[]), () => typeof(StreamHelper).GetMethod("ReadCharArray"));
            DefaultGetterMethods.Add(typeof(DateTime), () => typeof(IDataRecord).GetMethod("GetDateTime"));
            DefaultGetterMethods.Add(typeof(decimal), () => typeof(IDataRecord).GetMethod("GetDecimal"));
            DefaultGetterMethods.Add(typeof(double), () => typeof(IDataRecord).GetMethod("GetDouble"));
            DefaultGetterMethods.Add(typeof(float), () => typeof(IDataRecord).GetMethod("GetFloat"));
            DefaultGetterMethods.Add(typeof(Guid), () => typeof(IDataRecord).GetMethod("GetGuid"));
            DefaultGetterMethods.Add(typeof(short), () => typeof(IDataRecord).GetMethod("GetInt16"));
            DefaultGetterMethods.Add(typeof(int), () => typeof(IDataRecord).GetMethod("GetInt32"));
            DefaultGetterMethods.Add(typeof(long), () => typeof(IDataRecord).GetMethod("GetInt64"));
            DefaultGetterMethods.Add(typeof(string), () => typeof(IDataRecord).GetMethod("GetString"));
        }
    }
}