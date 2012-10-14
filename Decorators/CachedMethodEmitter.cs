namespace DbExtensions.Decorators
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    using DbExtensions.Interfaces;

    public class CachedMethodEmitter<T> : IMethodEmitter<T> 
    {
        private readonly IMethodEmitter<T> methodEmitter;

        private readonly ConcurrentDictionary<Type, Func<IDataRecord, int[], T>> cache 
            = new ConcurrentDictionary<Type,Func<IDataRecord,int[],T>>();

        public CachedMethodEmitter(IMethodEmitter<T> methodEmitter)
        {
            this.methodEmitter = methodEmitter;
        }

        public Func<IDataRecord, int[], T> CreateMethod(Type type)
        {
            return cache.GetOrAdd(type, t => methodEmitter.CreateMethod(t));
        }
    }
}