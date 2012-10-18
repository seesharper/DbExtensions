namespace DbExtensions.Interfaces
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq.Expressions;

    public interface IInstanceExpressionBuilder<T>
    {
        Expression<Func<IDataRecord, T>> CreateExpression(IDataRecord dataRecord);
    }

    public class InstanceExpressionBuilder<T> : IInstanceExpressionBuilder<T>
    {
        private readonly IMapper<T> instanceEmitter;

        private readonly IOrdinalSelector ordinalSelector;

        private readonly IManyToOneExpressionBuilder manyToOneExpressionBuilder;

        public InstanceExpressionBuilder(IMapper<T> instanceEmitter, IOrdinalSelector ordinalSelector, IManyToOneExpressionBuilder manyToOneExpressionBuilder)
        {
            this.instanceEmitter = instanceEmitter;
            this.ordinalSelector = ordinalSelector;
            this.manyToOneExpressionBuilder = manyToOneExpressionBuilder;
        }

        public Expression<Func<IDataRecord, T>> CreateExpression(IDataRecord dataRecord)
        {
            int[] ordinals = ordinalSelector.Execute(typeof(T), dataRecord);
            Func<IDataRecord, int[], T> createInstanceMethod = instanceEmitter.CreateMethod(typeof(T));
            var ordinalsExpression = Expression.Constant(ordinals,typeof(int[]));
            var createInstanceMethodExpression = Expression.Constant(createInstanceMethod, typeof(Func<IDataRecord, int[], T>));
            ParameterExpression datarecordExpression = Expression.Parameter(typeof(IDataRecord), "dataRecord");
            var invokeExpression = Expression.Invoke(createInstanceMethodExpression, new Expression[] { datarecordExpression, ordinalsExpression });
            ParameterExpression instanceExpression = Expression.Variable(typeof(T), "instance");
            BinaryExpression assignExpression = Expression.Assign(instanceExpression, invokeExpression);
            Expression<Action<IDataRecord, T>> manyToOneExpression = manyToOneExpressionBuilder.CreateExpression<T>(dataRecord);
            BlockExpression blockExpression;
            if (manyToOneExpression != null)
            {
                blockExpression = Expression.Block(new ParameterExpression[] { instanceExpression }, assignExpression, CreateInvocationExpression(manyToOneExpression, datarecordExpression, instanceExpression), instanceExpression);
            }
            else
            {
                blockExpression = Expression.Block(new ParameterExpression[] { instanceExpression }, assignExpression);
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