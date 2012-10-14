namespace DbExtensions.Interfaces
{
    using System.Data;

    public interface IDataRecordMapper<T>
        where T : class
    {
        T Execute(IDataRecord dataRecord);
    }
}