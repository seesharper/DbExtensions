namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections;
    using System.Data;

    public interface IInstanceDelegateBuilder<T> where T:class
    {
        Func<IDataRecord, T> CreateInstanceDelegate();
    }

    public class InstanceDelegateBuilder<T> : IInstanceDelegateBuilder<T> where T : class
    {
        private readonly IMapper<T> instanceEmitter;

        private readonly IOrdinalSelector ordinalSelector;

        private readonly IManyToOneDelegateBuilder<T> manyToOneDelegateBuilder;

        public InstanceDelegateBuilder(IMapper<T> instanceEmitter, IOrdinalSelector ordinalSelector, IManyToOneDelegateBuilder<T> manyToOneDelegateBuilder )
        {
            this.instanceEmitter = instanceEmitter;
            this.ordinalSelector = ordinalSelector;
            this.manyToOneDelegateBuilder = manyToOneDelegateBuilder;
        }

        public Func<IDataRecord, T> CreateInstanceDelegate()
        {
            return CreateInstance;
        }

        private T CreateInstance(IDataRecord dataRecord)
        {
            T instance = instanceEmitter.CreateMethod(typeof(T))(dataRecord, ordinalSelector.Execute(typeof(T), dataRecord));            
            var manyToOneDelegate = GetManyToOneDelegate(dataRecord);
            if (manyToOneDelegate != null)
            {
                manyToOneDelegate(dataRecord, instance);
            }

            return instance;
        }

        private Action<IDataRecord, T> GetManyToOneDelegate(IDataRecord dataRecord)
        {
            return manyToOneDelegateBuilder.CreateDelegate(dataRecord);
        }
    }
}