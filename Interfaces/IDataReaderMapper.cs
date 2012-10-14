namespace DbExtensions.Interfaces
{
    using System.Collections.Generic;
    using System.Data;

    public interface IDataReaderMapper<T> where T: class
    {
        IEnumerable<T> Execute(IDataReader dataReader);
    }
}