namespace DbExtensions.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using DbExtensions.Interfaces;
    using DbExtensions.Model;

    /// <summary>
    /// A class that creates a delegate that is capable of retrieving key values 
    /// from a <see cref="IDataRecord"/> based on a <see cref="Type"/>.
    /// </summary>
    public class KeyDelegateBuilder : IKeyDelegateBuilder
    {
        private readonly IPropertyMapper propertyMapper;
        private readonly IMapperDelegateBuilder<IStructuralEquatable> keyInstanceEmitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDelegateBuilder"/> class.
        /// </summary>
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> responsible for mapping fields/columns to properties.</param>
        /// <param name="keyInstanceEmitter">The <see cref="IMapperDelegateBuilder{T}"/> that is responsible for emitting a method that 
        /// populates a <see cref="IStructuralEquatable"/> with key values.</param>
        public KeyDelegateBuilder(IPropertyMapper propertyMapper, IMapperDelegateBuilder<IStructuralEquatable> keyInstanceEmitter)
        {
            this.propertyMapper = propertyMapper;
            this.keyInstanceEmitter = keyInstanceEmitter;
        }

        /// <summary>
        /// Creates a function delegate that returns a <see cref="IStructuralEquatable"/> instance, 
        /// representing the key values for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to build the delegate.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> containing the target fields/columns.</param>
        /// <returns>A function delegate used to retrieve key values for the given <paramref name="type"/>.</returns>
        public Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(Type type, IDataRecord dataRecord)
        {
            IEnumerable<PropertyMappingInfo> ordinals = propertyMapper.Execute(type, dataRecord);            
            var keyOrdinal = GetValidPropertyMappingWithTheSmallestOrdinal(ordinals);
            if (keyOrdinal == null)
            {
                return null;
            }

            Type keyType = typeof(Tuple<>).MakeGenericType(keyOrdinal.Property.PropertyType);            
            var keyMethod = keyInstanceEmitter.CreateMethod(keyType);            
            return dr => keyMethod(dr, new[] { keyOrdinal.Ordinal });
        }

        private static PropertyMappingInfo GetValidPropertyMappingWithTheSmallestOrdinal(IEnumerable<PropertyMappingInfo> ordinals)
        {
            return ordinals.Where(pm => pm.Ordinal != -1).OrderBy(pm => pm.Ordinal).FirstOrDefault();
        }
    }    
}