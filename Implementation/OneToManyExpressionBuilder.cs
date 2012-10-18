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

    /// <summary>
    /// A class that is capable of creating an expression that maps one to many relations.
    /// </summary>
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
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> that is responsible for mapping fields/columns from an <see cref="IDataRecord"/> to
        /// the properties of a <see cref="Type"/>.</param>
        /// <param name="collectionPropertySelector">The <see cref="IPropertySelector"/> that is responsible for selecting collection properties from a given <see cref="Type"/>.</param>
        /// <param name="dataRecordMapperFactory">A function delegate used to create the <see cref="IDataReaderMapper{T}"/> needed for each collection property.</param>
        public OneToManyExpressionBuilder(IPropertyMapper propertyMapper, IPropertySelector collectionPropertySelector, Func<Type, object> dataRecordMapperFactory)
        {
            this.propertyMapper = propertyMapper;
            this.collectionPropertySelector = collectionPropertySelector;
            this.dataRecordMapperFactory = dataRecordMapperFactory;            
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that maps one to many relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> used to map one to many relations.</returns>
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

            return expressions.Count > 0 ? Expression.Lambda<Action<IDataRecord, T>>(Expression.Block(expressions), dataRecordParameter, instanceParameter) : null;
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
            MethodCallExpression addMethodCallExpression = Expression.Call(collectionPropertyExpression, addMethod, new Expression[] { elementConstantExpression });
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
            return Expression.Call(dataRecordMapperConstantExpression, executeMethod, new Expression[] { dataRecordParameter });
        }

        private bool HasAtLeastOneMappedProperty(Type type, IDataRecord dataRecord)
        {
            return propertyMapper.Execute(type, dataRecord).Any(pm => pm.Ordinal > -1);
        }
    }
}