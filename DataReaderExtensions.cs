namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using DbExtensions.Core;
    using DbExtensions.Interfaces;

    /// <summary>
    /// Extends the <see cref="IDataReader"/> interface.
    /// </summary>
    public static class DataReaderExtensions
    {
        private static readonly IServiceContainer ServiceContainer = new ServiceContainer();
        
        static DataReaderExtensions()
        {
            ServiceContainer.RegisterAssembly(typeof(DataReaderExtensions).Assembly, ShouldRegister());
        }

        public static void RegisterMethodSelector<TSelector>() where TSelector : IMethodSelector
        {
            ServiceContainer.Register<IMethodSelector, TSelector>();
        }

        /// <summary>
        /// Transforms the <paramref name="dataReader"/> into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to be created from the <paramref name="dataReader"/>.</typeparam>
        /// <param name="dataReader">The target <see cref="IDataReader"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that represents the result of transforming the data reader.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this IDataReader dataReader) where T : class
        {
            using (ServiceContainer.BeginResolutionScope())
            {
                var dataReaderMapper = ServiceContainer.GetInstance<IDataReaderMapper<T>>();                
                return dataReaderMapper.Execute(dataReader);                
            }
        }

        /// <summary>
        /// Transforms the <paramref name="dataReader"/> into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to be created from the <paramref name="dataReader"/>.</typeparam>
        /// <param name="dataReader">The target <see cref="IDataReader"/>.</param>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        public static T As<T>(this IDataReader dataReader) where T : class
        {
            return dataReader.AsEnumerable<T>().FirstOrDefault();
        }

        private static Func<Type, bool> ShouldRegister()
        {
            return t => t.Namespace.Contains("DbExtensions");
        }
    }
}