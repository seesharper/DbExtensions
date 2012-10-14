namespace DbExtensions.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    using DbExtensions.Interfaces;

    public class DataRecordMapper<T> : IDataRecordMapper<T>
        where T : class
    {        
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IKeyDelegateBuilder keyDelegateBuilder;
        private readonly IOneToManyDelegateBuilder oneToManyDelegateBuilder;

        private readonly IManyToOneDelegateBuilder manyToOneDelegateBuilder;

        private readonly ConcurrentDictionary<IStructuralEquatable, T> queryCache = new ConcurrentDictionary<IStructuralEquatable, T>();
        private readonly Func<IDataRecord, int[], T> createInstance;
            
        public DataRecordMapper(IOrdinalSelector ordinalSelector, IMethodEmitter<T> instanceEmitter, IKeyDelegateBuilder keyDelegateBuilder, IOneToManyDelegateBuilder oneToManyDelegateBuilder, IManyToOneDelegateBuilder manyToOneDelegateBuilder)
        {        
            createInstance = instanceEmitter.CreateMethod(typeof(T));
            this.ordinalSelector = ordinalSelector;
            this.keyDelegateBuilder = keyDelegateBuilder;
            this.oneToManyDelegateBuilder = oneToManyDelegateBuilder;
            this.manyToOneDelegateBuilder = manyToOneDelegateBuilder;
        }

        public T Execute(IDataRecord dataRecord)
        {                        
            T instance = GetInstance(dataRecord, ordinalSelector.Execute(typeof(T), dataRecord));
            ExecuteChildDelegate(dataRecord, instance);
            return instance;
        }

        private void ExecuteChildDelegate(IDataRecord dataRecord, T instance)
        {
            var oneToManyDelegate = GetOneToManyDelegate(dataRecord);
            if (oneToManyDelegate != null)
            {
                oneToManyDelegate(dataRecord, instance);
            }
        }

        private T GetInstance(IDataRecord dataRecord, int[] ordinals)
        {
            var key = GetKey(dataRecord);
            return key == null ? null : queryCache.GetOrAdd(key, k => CreateInstance(dataRecord, ordinals));
        }

        private IStructuralEquatable GetKey(IDataRecord dataRecord)
        {
            Func<IDataRecord, IStructuralEquatable> keyDelegate = CreateKeyDelegate(dataRecord);
            
            if (keyDelegate == null)
            {
                return null;
            }
            
            var key = keyDelegate(dataRecord);
            return key;
        }

        private Action<IDataRecord, T> GetOneToManyDelegate(IDataRecord dataRecord)
        {
            return oneToManyDelegateBuilder.CreateDelegate<T>(dataRecord);            
        }

        private Action<IDataRecord, T> GetManyToOneDelegate(IDataRecord dataRecord)
        {
            return manyToOneDelegateBuilder.CreateDelegate<T>(dataRecord);
        }

        private Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(IDataRecord dataRecord)
        {
            return keyDelegateBuilder.CreateKeyDelegate(typeof(T), dataRecord);            
        }
        
        private T CreateInstance(IDataRecord dataRecord, int[] ordinals)
        {
            T instance = createInstance(dataRecord, ordinals);
            var manyToOneDelegate = GetManyToOneDelegate(dataRecord);
            if (manyToOneDelegate != null)
            {
                manyToOneDelegate(dataRecord, instance);
            }
            return instance;
        }
    }    
}