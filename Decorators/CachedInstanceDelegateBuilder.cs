namespace DbExtensions.Decorators
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;

    public class CachedInstanceDelegateBuilder<T> : IInstanceDelegateBuilder<T> where T :class
    {
        private readonly IInstanceDelegateBuilder<T> instanceDelegateBuilder;

        private Func<IDataRecord, T> cachedDelegate;

        public CachedInstanceDelegateBuilder(IInstanceDelegateBuilder<T> instanceDelegateBuilder )
        {
            this.instanceDelegateBuilder = instanceDelegateBuilder;
        }

        public Func<IDataRecord, T> CreateInstanceDelegate()
        {
            if (cachedDelegate == null)
            {
                cachedDelegate = instanceDelegateBuilder.CreateInstanceDelegate();
            }
            return cachedDelegate;
        }
    }
}