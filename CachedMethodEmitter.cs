namespace DbExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    public class CachedMethodEmitter<T> : IMethodEmitter<T> 
    {
        private readonly IMethodEmitter<T> methodEmitter;

        private Lazy<Func<IDataRecord, int[], T>> cache;

        public CachedMethodEmitter(IMethodEmitter<T> methodEmitter)
        {            
            cache = new Lazy<Func<IDataRecord, int[], T>>(() => methodEmitter.CreateMethod(typeof(T)));
        }

        public Func<IDataRecord, int[], T> CreateMethod(Type type)
        {
            return cache.Value;
        }
    }
}