namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;

    public interface IOrdinalSelector
    {
        int[] Execute(Type type, IDataRecord dataRecord);
    }

    //public class CachedOrdinalSelector : IOrdinalSelector
    //{
    //    private readonly IOrdinalSelector ordinalSelector;
    //    private ConcurrentDictionary<,> 

    //    public CachedOrdinalSelector(IOrdinalSelector ordinalSelector)
    //    {
    //        this.ordinalSelector = ordinalSelector;
    //    }

    //    public int[] Execute(Type type, IDataRecord dataRecord)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}