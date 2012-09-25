namespace DbExtensions
{
    using System.Collections.Generic;
    using System.Data;

    public static class DataReaderExtensions
    {
         public static IEnumerable<T> AsEnumerable<T>(this IDataReader dataReader)
         {
             return null;
         }
    }
}