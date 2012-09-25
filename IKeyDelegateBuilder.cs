namespace DbExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    
    public interface IKeyDelegateBuilder
    {
        Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(Type type, IDataRecord dataRecord);
    }

    public class KeyDelegateBuilder : IKeyDelegateBuilder
    {
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IMethodEmitter<IStructuralEquatable> methodEmitter;

        public KeyDelegateBuilder(IOrdinalSelector ordinalSelector, IMethodEmitter<IStructuralEquatable> methodEmitter)
        {
            this.ordinalSelector = ordinalSelector;
            this.methodEmitter = methodEmitter;
        }

        public Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(Type type, IDataRecord dataRecord)
        {
            IEnumerable<PropertyMappingInfo> ordinals = ordinalSelector.Execute(type, dataRecord.GetAllNames());
            var keyOrdinal = ordinals.OrderBy(pm => pm.Ordinal).FirstOrDefault();
            if (keyOrdinal != null && keyOrdinal.Ordinal == -1)
            {
                throw new InvalidOperationException("Unable to create key for type: ".FormatWith(type.ToString()));
            }
            Type keyType = typeof(Tuple<>).MakeGenericType(keyOrdinal.Property.PropertyType);            
            var keyMethod = (Func<IDataRecord, int[], IStructuralEquatable>)methodEmitter.CreateMethod(keyType);            
            return dr => keyMethod(dr, new[] { keyOrdinal.Ordinal });
        }
    }
}