namespace DbExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class DataRecordMapper<T> where T : class
    {
        private readonly IOrdinalSelector ordinalSelector;
        private readonly IMethodEmitter propertyEmitter;

        


        private static ParameterExpression instanceParameter;

        private static readonly ParameterExpression dataRecordParameter;

        private Action<IDataRecord, T> listDelegate;

        static DataRecordMapper() 
        {
            instanceParameter = Expression.Parameter(typeof(T), "instance");
            dataRecordParameter = Expression.Parameter(typeof(IDataRecord), "dataRecord");
        }

        public DataRecordMapper(IOrdinalSelector ordinalSelector, IMethodEmitter propertyEmitter)
        {
            this.ordinalSelector = ordinalSelector;
            this.propertyEmitter = propertyEmitter;        
        }

        public DataRecordMapper()
        {
            IPropertySelector propertySelector = new SimplePropertySelector();
            this.propertyEmitter = new PropertyBasedMethodEmitter(new DynamicMethodSkeleton(typeof(T), new[] { typeof(IDataRecord), typeof(int[]) }), new MethodSelector(), propertySelector); 
            this.ordinalSelector = new OrdinalSelector(propertySelector);
        }

        public T Execute(IDataRecord dataRecord)
        {
            this.CreateKeyDelegate(dataRecord);
            
            var instance = CreateDelegate(dataRecord)(dataRecord);
            if (listDelegate != null)
            {
                listDelegate(dataRecord, instance);
            }
            return instance;
        }

        private Func<IDataRecord, IStructuralEquatable> CreateKeyDelegate(IDataRecord dataRecord)
        {
            
            IEnumerable<PropertyMappingInfo> ordinals = ordinalSelector.Execute(typeof(T), dataRecord.GetAllNames());
            var keyOrdinal = ordinals.OrderBy(pm => pm.Ordinal).FirstOrDefault();

            Type keyType = typeof(Tuple<>).MakeGenericType(keyOrdinal.Property.PropertyType);

            var keyEmitter = new ConstructorBasedMethodEmitter(new DynamicMethodSkeleton(keyType, new[] { typeof(IDataRecord), typeof(int[]) }), new MethodSelector(), new ConstructorSelector());
            var keyMethod = (Func<IDataRecord, int[], IStructuralEquatable>)keyEmitter.CreateMethod(keyType);
            var key = keyMethod(dataRecord, new int[] { keyOrdinal.Ordinal });
            return null;
        }


        public Func<IDataRecord, T> CreateDelegate(IDataRecord dataRecord)
        {
            IDictionary<string, int> columnOrdinals = dataRecord.GetAllNames();
            int[] ordinals = this.ordinalSelector.Execute(typeof(T), columnOrdinals).Select(pm => pm.Ordinal).ToArray();
            CreateMethodCallExpressions();
            var method = (Func<IDataRecord, int[], T>)this.propertyEmitter.CreateMethod(typeof(T));
            return record => method(record, ordinals);
        }

        private void CreateMethodCallExpressions()
        {
            PropertyInfo[] collectionProperties = this.GetCollectionProperties();
            if (collectionProperties.Length > 0)
            {
                List<Expression> expressions = new List<Expression>();
                foreach (PropertyInfo collectionProperty in collectionProperties)
                {
                    expressions.Add(CreateAddMethodCallExpression(collectionProperty));
                }
                var blockExpression = Expression.Block(expressions);
                var lambdaExpression = Expression.Lambda<Action<IDataRecord, T>>(blockExpression, dataRecordParameter, instanceParameter);
                listDelegate = lambdaExpression.Compile();
            }

        }

        private MethodCallExpression CreateAddMethodCallExpression(PropertyInfo collectionProperty)
        {
            Type collectionInterfaceType = GetCollectionInterfaceType(collectionProperty.PropertyType);
            Type itemType = collectionInterfaceType.GetGenericArguments()[0];
            Type dataRecordMapperType = typeof(DataRecordMapper<>).MakeGenericType(itemType);
            object dataRecordMapper = Activator.CreateInstance(dataRecordMapperType);
            MethodInfo executeMethod = dataRecordMapperType.GetMethod("Execute");
            MethodInfo addMethod = collectionInterfaceType.GetMethod("Add");
            MemberExpression collectionPropertyExpression = Expression.MakeMemberAccess(instanceParameter, collectionProperty);
            MethodCallExpression executeMethodCallExpression = Expression.Call(Expression.Constant(dataRecordMapper), executeMethod, dataRecordParameter);
            MethodCallExpression addMethodCallExpression = Expression.Call(collectionPropertyExpression, addMethod, executeMethodCallExpression);
            return addMethodCallExpression;
        }


        private PropertyInfo[] GetCollectionProperties()
        {
            return typeof(T).GetProperties().Where(IsCollectionProperty).ToArray();
        }

        private bool IsCollectionProperty(PropertyInfo propertyInfo)
        {
            return GetCollectionInterfaceType(propertyInfo.PropertyType) != null;
        }

        private static Type GetCollectionInterfaceType(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return interfaceType;
                }
            }
            return null;
        }
    }

    
}