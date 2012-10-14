namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    public interface IOneToManyDelegateBuilder
    {
        Action<IDataRecord, T> CreateDelegate<T>(IDataRecord dataRecord);
    }
}