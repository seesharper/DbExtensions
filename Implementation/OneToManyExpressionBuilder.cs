namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using DbExtensions.Interfaces;

    public class OneToManyExpressionBuilder : IOneToManyExpressionBuilder
    {
        private readonly IPropertyMapper propertyMapper;
        private readonly IPropertySelector collectionPropertySelector;

        private readonly Func<Type, object> dataRecordMapperFactory;

        private ParameterExpression instanceParameter;
        private ParameterExpression dataRecordParameter;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyExpressionBuilder"/> class.
        /// </summary>
        /// <param name="propertyMapper"></param>
        /// <param name="collectionPropertySelector"></param>
        /// <param name="dataRecordMapperFactory"></param>
        public OneToManyExpressionBuilder(IPropertyMapper propertyMapper, IPropertySelector collectionPropertySelector, Func<Type,object> dataRecordMapperFactory)
        {
            this.propertyMapper = propertyMapper;
            this.collectionPropertySelector = collectionPropertySelector;
            this.dataRecordMapperFactory = dataRecordMapperFactory;            
        }

        public Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord)
        {
            instanceParameter = Expression.Parameter(typeof(T), "instance");
            dataRecordParameter = Expression.Parameter(typeof(IDataRecord), "dataRecord");
            PropertyInfo[] collectionProperties = collectionPropertySelector.Execute(typeof(T));
            var expressions = new Collection<Expression>();
            foreach (PropertyInfo property in collectionProperties)
            {                
                Type interfaceType = GetCollectionInterfaceType(property.PropertyType);
                Type elementType = interfaceType.GetGenericArguments()[0];
                if (HasAtLeastOneMappedProperty(elementType, dataRecord))
                {
                    expressions.Add(CreateAddMethodCallExpression(interfaceType, elementType, property));
                }
            }

            return expressions.Count > 0 ? Expression.Lambda<Action<IDataRecord, T>>(Expression.Block(expressions), this.dataRecordParameter, this.instanceParameter) : null;
        }

        private static Type GetCollectionInterfaceType(Type type)
        {
            return type.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        private Expression CreateAddMethodCallExpression(Type interfaceType, Type elementType, PropertyInfo propertyInfo)
        {
            Type dataRecordMapperType = typeof(IDataRecordMapper<>).MakeGenericType(elementType);
            MemberExpression collectionPropertyExpression = CreatePropertyMemberExpression(propertyInfo);
            MethodInfo addMethod = interfaceType.GetMethod("Add");
            MethodCallExpression executeMethodCallExpression = CreateExecuteMethodCallExpression(dataRecordMapperType);
            ParameterExpression elementConstantExpression = Expression.Variable(elementType, "childInstance");
            BinaryExpression assignExpression = Expression.Assign(elementConstantExpression, executeMethodCallExpression); 
            MethodCallExpression addMethodCallExpression = Expression.Call(collectionPropertyExpression, addMethod, elementConstantExpression);
            BinaryExpression nullCheckExpression = Expression.MakeBinary(ExpressionType.NotEqual, elementConstantExpression, Expression.Constant(null));
            return Expression.Block(new[] { elementConstantExpression }, assignExpression, Expression.IfThen(nullCheckExpression, addMethodCallExpression));



        }

        private ConstantExpression CreateDataRecordMapperConstantExpression(Type dataRecordMapperType)
        {           
            return Expression.Constant(dataRecordMapperFactory(dataRecordMapperType));
        }

        private MemberExpression CreatePropertyMemberExpression(PropertyInfo collectionProperty)
        {
            return Expression.MakeMemberAccess(instanceParameter, collectionProperty);
        }

        private MethodCallExpression CreateExecuteMethodCallExpression(Type dataRecordMapperType)
        {
            MethodInfo executeMethod = dataRecordMapperType.GetMethod("Execute");
            ConstantExpression dataRecordMapperConstantExpression = CreateDataRecordMapperConstantExpression(dataRecordMapperType);
            return Expression.Call(dataRecordMapperConstantExpression, executeMethod, dataRecordParameter);
        }

        private bool HasAtLeastOneMappedProperty(Type type, IDataRecord dataRecord)
        {
            return propertyMapper.Execute(type, dataRecord).Any(pm => pm.Ordinal > -1);
        }
    }
}