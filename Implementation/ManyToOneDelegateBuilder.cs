namespace DbExtensions.Implementation
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;

    public class ManyToOneDelegateBuilder : IManyToOneDelegateBuilder
    {
        private readonly IManyToOneExpressionBuilder manyToOneExpressionBuilder;

        public ManyToOneDelegateBuilder(IManyToOneExpressionBuilder manyToOneExpressionBuilder)
        {
            this.manyToOneExpressionBuilder = manyToOneExpressionBuilder;
        }

        public Action<IDataRecord, T> CreateDelegate<T>(IDataRecord dataRecord)
        {
            var expression = manyToOneExpressionBuilder.CreateExpression<T>(dataRecord);
            return expression == null ? null : expression.Compile();
        }
    }
}