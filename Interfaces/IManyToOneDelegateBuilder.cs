namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    public interface IManyToOneDelegateBuilder
    {
        Action<IDataRecord, T> CreateDelegate<T>(IDataRecord dataRecord);
    }
}