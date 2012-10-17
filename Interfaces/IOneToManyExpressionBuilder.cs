namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a class that is capable of creating an expression that maps one to many relations.
    /// </summary>
    public interface IOneToManyExpressionBuilder
    {
        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that maps one to many relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> used to map one to many relations.</returns>
        Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord);
    }
}