namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    public interface IManyToOneExpressionBuilder
    {
        Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord); 
    }
}