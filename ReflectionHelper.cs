namespace DbExtensions
{
    using System.Data;
    using System.Reflection;

    internal static class ReflectionHelper
    {
        static ReflectionHelper()
        {
            IsDbNullMethod = typeof(IDataRecord).GetMethod("IsDBNull");
        }

        public static MethodInfo IsDbNullMethod { get; private set; }
        
        

    }
}