namespace DbExtensions.Implementation
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;

    public class OneToManyDelegateBuilder : IOneToManyDelegateBuilder
    {
        private readonly IOneToManyExpressionBuilder oneToManyExpressionBuilder;

        public OneToManyDelegateBuilder(IOneToManyExpressionBuilder oneToManyExpressionBuilder)
        {
            this.oneToManyExpressionBuilder = oneToManyExpressionBuilder;
        }

        public Action<IDataRecord, T> CreateDelegate<T>(IDataRecord dataRecord)
        {
            var expression = oneToManyExpressionBuilder.CreateExpression<T>(dataRecord);
            return expression == null ? null : expression.Compile();
        }
    }
}