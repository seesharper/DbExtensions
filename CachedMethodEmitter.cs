namespace DbExtensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Data;

    public class CachedMethodEmitter : IMethodEmitter 
    {
        private readonly IMethodEmitter methodEmitter;

        private readonly ConcurrentDictionary<Type, Delegate> cache = new ConcurrentDictionary<Type, Delegate>();

        public CachedMethodEmitter(IMethodEmitter methodEmitter)
        {
            this.methodEmitter = methodEmitter;
        }

        public Delegate CreateMethod(Type type)
        {
            return cache.GetOrAdd(type, t => this.methodEmitter.CreateMethod(t));
        }
    }
}