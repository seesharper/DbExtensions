namespace DbExtensions.Implementation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that is capable of creating an expression that maps many to one relations.
    /// </summary>
    public class ManyToOneExpressionBuilder : IManyToOneExpressionBuilder
    {
        private readonly IPropertyMapper propertyMapper;

        private readonly IPropertySelector complexPropertySelector;

        private readonly Func<Type, object> dataRecordMapperFactory;

        private ParameterExpression instanceParameter;
        private ParameterExpression dataRecordParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManyToOneExpressionBuilder"/> class.
        /// </summary>
        /// <param name="propertyMapper">The <see cref="IPropertyMapper"/> instance that is responsible for 
        /// mapping <see cref="IDataRecord"/> fields to properties.</param>
        /// <param name="complexPropertySelector">The <see cref="IPropertySelector"/> instance that is responsible for selecting 
        /// complex properties from a given <see cref="Type"/>.</param>
        /// <param name="dataRecordMapperFactory">A function delegate used to create <see cref="DataRecordMapper{T}"/></param>
        public ManyToOneExpressionBuilder(IPropertyMapper propertyMapper, IPropertySelector complexPropertySelector, Func<Type, object> dataRecordMapperFactory)
        {
            this.propertyMapper = propertyMapper;
            this.complexPropertySelector = complexPropertySelector;
            this.dataRecordMapperFactory = dataRecordMapperFactory;
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that maps many to one relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> used to map many to one relations.</returns>
        public Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord)
        {
            instanceParameter = Expression.Parameter(typeof(T), "instance");
            dataRecordParameter = Expression.Parameter(typeof(IDataRecord), "dataRecord");
            var complexProperties = complexPropertySelector.Execute(typeof(T));
            var expressions = new Collection<Expression>();
            foreach (var complexProperty in complexProperties)
            {
                if (HasAtLeastOneMappedProperty(complexProperty.PropertyType, dataRecord))
                {
                    expressions.Add(CreateAssignExpression(complexProperty));
                }
            }

            return expressions.Count > 0 ? Expression.Lambda<Action<IDataRecord, T>>(Expression.Block(expressions), dataRecordParameter, instanceParameter) : null;
        }

        private Expression CreateAssignExpression(PropertyInfo property)
        {
            Type dataRecordMapperType = typeof(IDataRecordMapper<>).MakeGenericType(property.PropertyType);
            MemberExpression propertyExpression = CreatePropertyMemberExpression(property);
            BinaryExpression assignExpression = Expression.Assign(propertyExpression, CreateExecuteMethodCallExpression(dataRecordMapperType));
            return assignExpression;
        }

        private MemberExpression CreatePropertyMemberExpression(PropertyInfo property)
        {
            return Expression.MakeMemberAccess(instanceParameter, property);
        }

        private MethodCallExpression CreateExecuteMethodCallExpression(Type dataRecordMapperType)
        {
            MethodInfo executeMethod = dataRecordMapperType.GetMethod("Execute");
            ConstantExpression dataRecordMapperConstantExpression = CreateDataRecordMapperConstantExpression(dataRecordMapperType);
            return Expression.Call(dataRecordMapperConstantExpression, executeMethod, dataRecordParameter);
        }

        private ConstantExpression CreateDataRecordMapperConstantExpression(Type dataRecordMapperType)
        {
            return Expression.Constant(dataRecordMapperFactory(dataRecordMapperType));
        }

        private bool HasAtLeastOneMappedProperty(Type type, IDataRecord dataRecord)
        {
            return propertyMapper.Execute(type, dataRecord).Any(pm => pm.Ordinal > -1);
        }
    }
}