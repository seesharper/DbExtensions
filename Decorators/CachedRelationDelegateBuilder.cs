namespace DbExtensions.Decorators
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;

    public class CachedRelationDelegateBuilder<T> : IRelationDelegateBuilder<T>
    {
        private readonly IRelationDelegateBuilder<T> relationDelegateBuilder;

        private Action<IDataRecord, T> cachedDelegate;

        public CachedRelationDelegateBuilder(IRelationDelegateBuilder<T> relationDelegateBuilder )
        {
            this.relationDelegateBuilder = relationDelegateBuilder;
        }

        public Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord)
        {
            if (cachedDelegate == null)
            {
                cachedDelegate = relationDelegateBuilder.CreateDelegate(dataRecord);
            }
            return cachedDelegate;
        }
    }
}