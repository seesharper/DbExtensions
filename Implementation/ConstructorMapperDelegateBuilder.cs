namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;

    using DbExtensions.Interfaces;

    /// <summary>
    /// An <see cref="IMethodEmitterEmitter"/> implementation that is capable of 
    /// creating a new instance of <typeparamref name="T"/> providing constructor arguments 
    /// from a <see cref="IDataRecord"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of object to create.</typeparam>
    public class ConstructorMapperDelegateBuilder<T> : MapperDelegateBuilder<T>
    {        
        private readonly IConstructorSelector constructorSelector;        
                
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorMapperDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="methodSkeleton">
        /// A <see cref="IMethodSkeleton{T}"/> implementation that 
        /// represents the method skeleton for which to emit the method body.
        /// </param>
        /// <param name="methodSelector">
        /// The <see cref="IMethodSelector"/> implementation that is responsible for providing a get method 
        /// that targets an <see cref="IDataRecord"/> instance.
        /// </param>
        /// <param name="constructorSelector">
        /// A <see cref="IConstructorSelector"/> that is responsible for selecting the constructor to be used.
        /// </param>        
        public ConstructorMapperDelegateBuilder(IMethodSkeleton<T> methodSkeleton, IMethodSelector methodSelector, IConstructorSelector constructorSelector)
            : base(methodSkeleton, methodSelector)
        {
            this.constructorSelector = constructorSelector;
        }

        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>      
        /// <param name="type">The target type for which to create the dynamic method.s</param>
        /// <returns>An function delegate used to invoke the method.</returns>
        public override Func<IDataRecord, int[], T> CreateMethod(Type type)
        {
            var constructor = GetConstructor(type);            
            LoadConstructorArguments(constructor);
            EmitNewInstance(constructor);
            EmitReturn();
            return CreateDelegate(type);
        }

        private void LoadDataRecordValue(ParameterInfo parameter, int index)
        {
            MethodInfo getMethod = MethodSelector.Execute(parameter.ParameterType);
            var tryLoadNullValue = DefineLabel();
            var end = DefineLabel();
            EmitCheckForValidOrdinal(index, tryLoadNullValue);
            EmitCheckForDbNull(index, tryLoadNullValue);
            EmitGetValue(index, getMethod, parameter.ParameterType);
            EmitGoto(end);
            MarkLabel(tryLoadNullValue);
            if (parameter.ParameterType.IsValueType)
            {
                if (parameter.ParameterType.IsNullable())
                {
                    var local = ILGenerator.DeclareLocal(parameter.ParameterType);
                    ILGenerator.Emit(OpCodes.Ldloca_S, local);
                    ILGenerator.Emit(OpCodes.Initobj, parameter.ParameterType);
                    ILGenerator.Emit(OpCodes.Ldloc, local);
                }
                else
                {
                    ILGenerator.ThrowException(typeof(InvalidOperationException));    
                }                
            }
            else
            {
                ILGenerator.Emit(OpCodes.Ldnull);
            }

            MarkLabel(end);
        }

        private void EmitGoto(Label falseLabel)
        {
            this.ILGenerator.Emit(OpCodes.Br_S, falseLabel);
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            return this.constructorSelector.Execute(type);
        }
        
        private void EmitNewInstance(ConstructorInfo constructor)
        {
            ILGenerator.Emit(OpCodes.Newobj, constructor);
        }

        private void LoadConstructorArguments(ConstructorInfo constructor)
        {            
            ParameterInfo[] parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                LoadDataRecordValue(parameters[i], i);
            }
        }        
    }
}