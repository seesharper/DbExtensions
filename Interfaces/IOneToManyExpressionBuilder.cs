namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    public interface IOneToManyExpressionBuilder
    {
        Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord);
    }
}