namespace DbExtensions.Implementation
{
    using System;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that is capable of creating a delegate that maps one to many relations.
    /// </summary>
    /// <typeparam name="T">The type of object who's relations will be assigned.</typeparam>
    public class OneToManyDelegateBuilder<T> : IRelationDelegateBuilder<T>
    {
        private readonly IOneToManyExpressionBuilder oneToManyExpressionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneToManyDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="oneToManyExpressionBuilder">The <see cref="IOneToManyExpressionBuilder"/> that is responsible for 
        /// creating an expression that maps one to many relations.</param>
        public OneToManyDelegateBuilder(IOneToManyExpressionBuilder oneToManyExpressionBuilder)
        {
            this.oneToManyExpressionBuilder = oneToManyExpressionBuilder;
        }

        /// <summary>
        /// Creates a delegate that maps one to many relations.
        /// </summary>
        /// <typeparam name="T">The type of object that owns the relations.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate used to map one to many relations.</returns>
        public Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord)
        {
            var expression = oneToManyExpressionBuilder.CreateExpression<T>(dataRecord);
            return expression == null ? null : expression.Compile();
        }
    }
}