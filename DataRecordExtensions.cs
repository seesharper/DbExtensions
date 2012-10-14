namespace DbExtensions
{
    using System.Data;
    using System.Linq;

    public static class DataRecordExtensions
    {        
        public static T As<T>(this IDataReader dataReader) where T:class
        {
            return dataReader.AsEnumerable<T>().FirstOrDefault();
        }
    }
}