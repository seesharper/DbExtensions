namespace DbExtensions.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// Transforms an <see cref="IDataRecord"/> into an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the mapper.</typeparam>
    public class DataRecordMapper<T> : IDataRecordMapper<T>
        where T : class
    {
        private readonly IInstanceDelegateBuilder<T> instanceDelegateBuilder;
        private readonly IKeyDelegateBuilder keyDelegateBuilder;
        private readonly IRelationDelegateBuilder<T> oneToManyDelegateBuilder;        
        private readonly ConcurrentDictionary<IStructuralEquatable, T> queryCache = new ConcurrentDictionary<IStructuralEquatable, T>();        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRecordMapper{T}"/> class.
        /// </summary>
        /// <param name="instanceDelegateBuilder">The <see cref="IInstanceDelegateBuilder{T}"/> that is responsible for creating a delegate 
        /// that is capable of producing an instance of <typeparamref name="T"/> based on the current <see cref="IDataRecord"/>.</param>
        /// <param name="keyDelegateBuilder">The <see cref="IKeyDelegateBuilder"/> that is responsible for creating a delegate
        /// that is capable of producing an <see cref="IStructuralEquatable"/> instance based on the current <see cref="IDataRecord"/>.</param>
        /// <param name="oneToManyDelegateBuilder">The <see cref="IRelationDelegateBuilder{T}"/> that is responsible for creating a delegate 
        /// that is capable of mapping one to many relations</param>         
        public DataRecordMapper(IInstanceDelegateBuilder<T> instanceDelegateBuilder, IKeyDelegateBuilder keyDelegateBuilder, IRelationDelegateBuilder<T> oneToManyDelegateBuilder)
        {
            this.instanceDelegateBuilder = instanceDelegateBuilder;
            this.keyDelegateBuilder = keyDelegateBuilder;
            this.oneToManyDelegateBuilder = oneToManyDelegateBuilder;         
        }

        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <returns>An instance of <typeparamref name="T"/> that represents the result of the mapping/transformation.</returns>
        public T Execute(IDataRecord dataRecord)
        {                        
            T instance = GetInstance(dataRecord);
            ExecuteOneToManyDelegate(dataRecord, instance);
            return instance;
        }

        private void ExecuteOneToManyDelegate(IDataRecord dataRecord, T instance)
        {
            var oneToManyDelegate = oneToManyDelegateBuilder.CreateDelegate(dataRecord); 
            if (oneToManyDelegate != null)
            {
                oneToManyDelegate(dataRecord, instance);
            }
        }

        private T GetInstance(IDataRecord dataRecord)
        {
            var key = GetKey(dataRecord);
            return key == null ? null : queryCache.GetOrAdd(key, k => CreateInstance(dataRecord));
        }

        private T CreateInstance(IDataRecord dataRecord)
        {
            return instanceDelegateBuilder.CreateInstanceDelegate(dataRecord)(dataRecord);
        }

        private IStructuralEquatable GetKey(IDataRecord dataRecord)
        {
            Func<IDataRecord, IStructuralEquatable> keyDelegate = keyDelegateBuilder.CreateKeyDelegate(typeof(T), dataRecord);
            
            if (keyDelegate == null)
            {
                return null;
            }
            
            var key = keyDelegate(dataRecord);
            return key;
        }           
    }    
}