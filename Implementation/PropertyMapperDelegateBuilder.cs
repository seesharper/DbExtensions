namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;

    using DbExtensions.Interfaces;

    /// <summary>
    /// An <see cref="IMapperDelegateBuilder{T}"/> implementation that is capable of 
    /// creating a new instance of <typeparamref name="T"/> and setting property values from 
    /// as <see cref="IDataRecord"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    public class PropertyMapperDelegateBuilder<T> : MapperDelegateBuilder<T> 
    {
        private readonly IPropertySelector propertySelector;

        private LocalBuilder instanceVariable;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapperDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeleton">
        /// A <see cref="IMethodSkeleton{T}"/> implementation that 
        /// represents the method skeleton for which to emit the method body.
        /// </param>
        /// <param name="methodSelector">
        /// The <see cref="IMethodSelector"/> implementation that is responsible for providing a get method 
        /// that targets an <see cref="IDataRecord"/> instance.
        /// </param>
        /// <param name="propertySelector">
        /// A <see cref="IPropertySelector"/> that is responsible for selecting the properties that will have its
        /// value read from an <see cref="IDataRecord"/> instance.
        /// </param>        
        public PropertyMapperDelegateBuilder(IMethodSkeleton<T> methodSkeleton, IMethodSelector methodSelector, IPropertySelector propertySelector)
            : base(methodSkeleton, methodSelector)
        {
            this.propertySelector = propertySelector;
        }

        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>      
        /// <param name="type">The target type for which to create the dynamic method.s</param>
        /// <returns>An function delegate used to invoke the method.</returns>
        public override Func<IDataRecord, int[], T> CreateMethod(Type type)
        {
            ConstructorInfo constructorInfo = GetParameterlessConstructor(type);
            EmitNewInstance(constructorInfo);
            EmitPropertySetters(type);
            LoadInstance();
            EmitReturn();            
            return CreateDelegate(type);
        }

        private static ConstructorInfo GetParameterlessConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        private void LoadDataRecordValue(PropertyInfo propertyInfo, int index)
        {
            MethodInfo getMethod = MethodSelector.Execute(propertyInfo.PropertyType);
            var endLabel = DefineLabel();
            EmitCheckForValidOrdinal(index, endLabel);
            EmitCheckForDbNull(index, endLabel);
            LoadInstance();
            EmitGetValue(index, getMethod, propertyInfo.PropertyType);
            EmitCallPropertySetterMethod(propertyInfo);
            MarkLabel(endLabel);
        }
        
        private void EmitPropertySetters(Type type)
        {
            var properties = propertySelector.Execute(type);
            for (int i = 0; i < properties.Length; i++)
            {
                EmitPropertySetter(properties[i], i);
            }
        }

        private void EmitPropertySetter(PropertyInfo propertyInfo, int propertyIndex)
        {            
            LoadDataRecordValue(propertyInfo, propertyIndex);            
        }

        private void EmitCallPropertySetterMethod(PropertyInfo propertyInfo)
        {
            var setterMethod = propertyInfo.GetSetMethod();
            ILGenerator.Emit(OpCodes.Callvirt, setterMethod);
        }

        private void EmitNewInstance(ConstructorInfo constructorInfo)
        {
            ILGenerator.Emit(OpCodes.Newobj, constructorInfo);
            instanceVariable = ILGenerator.DeclareLocal(constructorInfo.DeclaringType);
            ILGenerator.Emit(OpCodes.Stloc, instanceVariable);
        }

        private void LoadInstance()
        {            
            ILGenerator.Emit(OpCodes.Ldloc, instanceVariable);
        }       
    }
}