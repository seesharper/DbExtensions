namespace DbExtensions.Interfaces
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a class that is capable of creating an expression that maps many to one relations.
    /// </summary>
    public interface IManyToOneExpressionBuilder
    {
        /// <summary>
        /// Creates an <see cref="Expression{TDelegate}"/> that maps many to one relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A <see cref="Expression{TDelegate}"/> used to map many to one relations.</returns>
        Expression<Action<IDataRecord, T>> CreateExpression<T>(IDataRecord dataRecord); 
    }
}