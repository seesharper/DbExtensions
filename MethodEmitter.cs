namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// A base class for <see cref="IMethodEmitterEmitter"/> implementations.
    /// </summary>
    /// <typeparam name="T">The type of object returned from the delegate produced by this <see cref="IMethodEmitterEmitter"/></typeparam>
    public abstract class MethodEmitter<T> : IMethodEmitter<T>
    {        
        private readonly IMethodSkeleton<T> methodSkeleton;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodEmitter{T}"/> class.
        /// </summary>
        /// <param name="methodSkeleton">
        /// The method skeleton.
        /// </param>
        /// <param name="methodSelector">
        /// The get method provider.
        /// </param>
        protected MethodEmitter(IMethodSkeleton<T> methodSkeleton, IMethodSelector methodSelector)
        {
            MethodSelector = methodSelector;
            this.methodSkeleton = methodSkeleton;            
            ILGenerator = methodSkeleton.GetILGenerator();
        }

        /// <summary>
        /// Gets the <see cref="IMethodSelector"/> used to provide the get method to be used. 
        /// </summary>
        protected IMethodSelector MethodSelector { get; private set; }

        /// <summary>
        /// Gets the <see cref="ILGenerator"/> instance for the dynamic method.
        /// </summary>
        protected ILGenerator ILGenerator { get; private set; }

        /// <summary>
        /// Creates a new method used to populate an object from an <see cref="IDataRecord"/>.
        /// </summary>        
        /// <returns>An function delegate used to invoke the method.</returns>
        public abstract Func<IDataRecord, int[], T> CreateMethod(Type type);
        
        /// <summary>
        /// Emits a <see cref="OpCodes.Ret"/> instruction into the current method body. 
        /// </summary>
        protected void EmitReturn()
        {
            ILGenerator.Emit(OpCodes.Ret);
        }
        
        /// <summary>
        /// Creates a delegate used to invoke the dynamic method.
        /// </summary>
        /// <returns>A function delegate point to the dynamic method.</returns>
        protected Func<IDataRecord, int[], T> CreateDelegate(Type type)
        {            
            return methodSkeleton.CreateDelegate();
        }

        /// <summary>
        /// Returns a <see cref="MethodInfo"/> that targets an <see cref="IDataRecord"/> instance based on the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to return a getter <see cref="MethodInfo"/>/>.</param>
        /// <returns>A <see cref="MethodInfo"/> that represents getting a value from an <see cref="IDataRecord"/>.</returns>
        protected MethodInfo GetGetterMethod(Type type)
        {
            return MethodSelector.Execute(type);
        }

        /// <summary>
        /// Emits a check to see if the current ordinal equals -1.
        /// </summary>
        /// <param name="index">The index of the ordinal to check.</param>
        /// <param name="trueLabel">The <see cref="Label"/> that represents where to jump if the ordinal value equals -1.</param>
        protected void EmitCheckForValidOrdinal(int index, Label trueLabel)
        {
            LoadOrdinal(index);
            LoadIntegerValueOfMinusOne();
            EmitCompareValues();
            EmitGotoEndLabelIfValueIsTrue(trueLabel);
        }

        /// <summary>
        /// Emits a check to see if the current value to be read from the <see cref="IDataRecord"/> is <see cref="DBNull"/>
        /// </summary>
        /// <param name="index">The index of the ordinal to check.</param>
        /// <param name="trueLabel">The <see cref="Label"/> that represents where to jump if 
        /// the value about to be read from the <see cref="IDataRecord"/> is <see cref="DBNull"/>.</param>
        protected void EmitCheckForDbNull(int index, Label trueLabel)
        {
            LoadDataRecord();
            LoadOrdinal(index);
            EmitCallIsDbNullMethod();
            EmitGotoEndLabelIfValueIsTrue(trueLabel);
        }

        /// <summary>
        /// Defines a new <see cref="Label"/> within the method body.
        /// </summary>
        /// <returns>A new <see cref="Label"/> instance.</returns>
        protected Label DefineLabel()
        {
            return ILGenerator.DefineLabel();
        }

        /// <summary>
        /// Marks a position within the method body with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> used to mark the position.</param>
        protected void MarkLabel(Label label)
        {
            ILGenerator.MarkLabel(label);
            }
                        
        private void LoadDataRecord()
        {
            ILGenerator.Emit(OpCodes.Ldarg_0);
        }

        private void LoadOrdinal(int index)
        {
            ILGenerator.Emit(OpCodes.Ldarg_1);
            ILGenerator.EmitFastInt(index);
            ILGenerator.Emit(OpCodes.Ldelem_I4);            
        }

        protected void EmitGetValue(int index, MethodInfo getMethod, Type targetType)
        {
            if(targetType.IsNullable())
            {
                EmitGetNullableValue(index, getMethod, targetType);
            }
            else
            {
                EmitGetNonNullableValue(index, getMethod, targetType);
            }
        }

        private void EmitGetNonNullableValue(int index, MethodInfo getMethod, Type targetType)
        {
            LoadDataRecord();
            LoadOrdinal(index);
            EmitCallGetMethod(getMethod);    
        }

        private void EmitGetNullableValue(int index, MethodInfo getMethod, Type targetType)
        {
            var local = ILGenerator.DeclareLocal(targetType);
            ILGenerator.Emit(OpCodes.Ldloca_S, local);
            LoadDataRecord();
            LoadOrdinal(index);
            EmitCallGetMethod(getMethod);
            var nullableConstructor = getMethod.ReturnType.GetNullableConstructor();
            ILGenerator.Emit(OpCodes.Call, nullableConstructor);
            ILGenerator.Emit(OpCodes.Ldloc, local);
        }

        private void EmitCallGetMethod(MethodInfo getMethod)
        {
            ILGenerator.Emit(getMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt, getMethod);
        }
        
        private void EmitCallIsDbNullMethod()
        {
            ILGenerator.Emit(OpCodes.Callvirt, ReflectionHelper.IsDbNullMethod);
        }

        private void EmitGotoEndLabelIfValueIsTrue(Label endLabel)
        {
            ILGenerator.Emit(OpCodes.Brtrue_S, endLabel);
        }

        private void EmitCompareValues()
        {
            ILGenerator.Emit(OpCodes.Ceq);
        }
               
        private void LoadIntegerValueOfMinusOne()
        {
            ILGenerator.Emit(OpCodes.Ldc_I4_M1);
        }

        
    }
}