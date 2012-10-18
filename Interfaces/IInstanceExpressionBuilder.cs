namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a class that is capable of creating an <see cref="Expression{TDelegate}"/> that 
    /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the <see cref="Expression{TDelegate}"/></typeparam>
    public interface IInstanceExpressionBuilder<T>
    {
        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that 
        /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that represents creating an instance of <typeparamref name="T"/>.</returns>
        Expression<Func<IDataRecord, T>> CreateExpression(IDataRecord dataRecord);
    }

    /// <summary>
    /// A class that is capable of creating an <see cref="Expression{TDelegate}"/> that 
    /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the <see cref="Expression{TDelegate}"/></typeparam>
    public class InstanceExpressionBuilder<T> : IInstanceExpressionBuilder<T>
    {
        private readonly IMapper<T> instanceEmitter;

        private readonly IOrdinalSelector ordinalSelector;

        private readonly IManyToOneExpressionBuilder manyToOneExpressionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceExpressionBuilder{T}"/> class.
        /// </summary>
        /// <param name="instanceEmitter">
        /// The <see cref="IMapper{T}"/> that is responsible for emitting a method that is capable of mapping a <see cref="IDataRecord"/> 
        /// to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <param name="ordinalSelector">
        /// The <see cref="IOrdinalSelector"/> that is responsible for providing a set of ordinal values for the public properties of <typeparamref name="T"/>.
        /// </param>
        /// <param name="manyToOneExpressionBuilder">
        /// The <see cref="IManyToOneExpressionBuilder"/> that is responsible for creating an <see cref="Expression{TDelegate}"/> that represents 
        /// mapping many to one relations.
        /// </param>
        public InstanceExpressionBuilder(IMapper<T> instanceEmitter, IOrdinalSelector ordinalSelector, IManyToOneExpressionBuilder manyToOneExpressionBuilder)
        {
            this.instanceEmitter = instanceEmitter;
            this.ordinalSelector = ordinalSelector;
            this.manyToOneExpressionBuilder = manyToOneExpressionBuilder;
        }

        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that 
        /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>An <see cref="Expression{TDelegate}"/> that represents creating an instance of <typeparamref name="T"/>.</returns>
        public Expression<Func<IDataRecord, T>> CreateExpression(IDataRecord dataRecord)
        {
            int[] ordinals = ordinalSelector.Execute(typeof(T), dataRecord);
            Func<IDataRecord, int[], T> createInstanceMethod = instanceEmitter.CreateMethod(typeof(T));
            var ordinalsExpression = Expression.Constant(ordinals, typeof(int[]));
            var createInstanceMethodExpression = Expression.Constant(createInstanceMethod, typeof(Func<IDataRecord, int[], T>));
            ParameterExpression datarecordExpression = Expression.Parameter(typeof(IDataRecord), "dataRecord");
            var invokeExpression = Expression.Invoke(createInstanceMethodExpression, new Expression[] { datarecordExpression, ordinalsExpression });
            ParameterExpression instanceExpression = Expression.Variable(typeof(T), "instance");
            BinaryExpression assignExpression = Expression.Assign(instanceExpression, invokeExpression);
            Expression<Action<IDataRecord, T>> manyToOneExpression = manyToOneExpressionBuilder.CreateExpression<T>(dataRecord);
            BlockExpression blockExpression;
            if (manyToOneExpression != null)
            {
                blockExpression = Expression.Block(new[] { instanceExpression }, assignExpression, CreateInvocationExpression(manyToOneExpression, datarecordExpression, instanceExpression), instanceExpression);
            }
            else
            {
                blockExpression = Expression.Block(new[] { instanceExpression }, assignExpression);
            }
            
            var lambda = Expression.Lambda<Func<IDataRecord, T>>(blockExpression, new[] { datarecordExpression });
            return lambda;
        }

        private InvocationExpression CreateInvocationExpression(Expression<Action<IDataRecord, T>> manyToOneExpression, ParameterExpression dataRecordExpression, ParameterExpression instanceExpression)
        {
            return Expression.Invoke(manyToOneExpression, new Expression[] { dataRecordExpression, instanceExpression });
        }
    }
}