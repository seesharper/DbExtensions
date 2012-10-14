namespace DbExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using DbExtensions.Core;
    using DbExtensions.Interfaces;

    public static class DataReaderExtensions
    {
        private static readonly IServiceContainer ServiceContainer = new ServiceContainer();
        
        static DataReaderExtensions()
        {
            ServiceContainer.RegisterAssembly(typeof(DataReaderExtensions).Assembly, ShouldRegister());
        }

        private static Func<Type, bool> ShouldRegister()
        {
            return t => t.Namespace.Contains("DbExtensions");
        }

        public static IEnumerable<T> AsEnumerable<T>(this IDataReader dataReader) where T:class
        {
            using (ServiceContainer.BeginResolutionScope())
            {
                var dataReaderMapper = ServiceContainer.GetInstance<IDataReaderMapper<T>>();                
                return dataReaderMapper.Execute(dataReader);                
            }
         }
    }
}