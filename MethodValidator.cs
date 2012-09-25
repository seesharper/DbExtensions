namespace DbExtensions
{
    using System;
    using System.Data;
    using System.Reflection;
    
    /// <summary>
    /// A decorator that provides validation capabilities to an <see cref="IMethodSelector"/> instance. 
    /// </summary>
    public class MethodValidator : IMethodSelector
    {
        private readonly IMethodSelector methodSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodValidator"/> class.
        /// </summary>
        /// <param name="methodSelector">
        /// The <see cref="IMethodSelector"/> responsible for selecting the appropriate <see cref="IDataRecord"/> get method. 
        /// </param>
        public MethodValidator(IMethodSelector methodSelector)
        {
            this.methodSelector = methodSelector;
        }

        /// <summary>
        /// Returns the appropriate get method for the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to return the get method.</param>
        /// <returns>A get method used to pull a value from an <see cref="IDataRecord"/> instance.</returns>
        public MethodInfo Execute(Type type)
        {
            MethodInfo methodInfo = methodSelector.Execute(type);
            Validate(methodInfo, type);
            return methodInfo;
        }

        private static void Validate(MethodInfo methodInfo, Type type)
        {
            if (MethodNotDeclaredByDataRecord(methodInfo))
            {
                if (!IsValidMethod(methodInfo))
                {
                    throw new InvalidOperationException(ErrorMessages.GetMethodHasInvalidSignature
                        .FormatWith(methodInfo, type.Name, methodInfo.Name));
                }
            }
        }

        private static bool MethodNotDeclaredByDataRecord(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType != typeof(IDataRecord);
        }

        private static bool IsValidMethod(MethodInfo methodInfo)
        {            
            return IsStaticMethod(methodInfo) && HasCorrectSignature(methodInfo);
        }

        private static bool IsStaticMethod(MethodInfo methodInfo)
        {
            return methodInfo.IsStatic;
        }

        private static bool HasCorrectSignature(MethodInfo methodInfo)
        {
            var methodParameters = methodInfo.GetParameters();
            return HasExactlyTwoParameters(methodParameters) && HasCorrectParameterTypes(methodParameters);
        }

        private static bool HasExactlyTwoParameters(ParameterInfo[] methodParameters)
        {
            return methodParameters.Length == 2;
        }

        private static bool HasCorrectParameterTypes(ParameterInfo[] methodParameters)
        {
            return FirstParameterIsDataRecord(methodParameters) && SecondParameterIsInt(methodParameters);
        }

        private static bool SecondParameterIsInt(ParameterInfo[] methodParameters)
        {
            return methodParameters[1].ParameterType != typeof(int);
        }

        private static bool FirstParameterIsDataRecord(ParameterInfo[] methodParameters)
        {
            return methodParameters[0].ParameterType != typeof(IDataRecord);
        }
    }
}