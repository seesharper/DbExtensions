namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that is capable of creating a delegate that maps many to one relations.
    /// </summary>
    /// <typeparam name="T">The type of object who's relations will be assigned.</typeparam>
    public class ManyToOneDelegateBuilder<T> : IManyToOneDelegateBuilder<T> 
    {
        private readonly IManyToOneExpressionBuilder manyToOneExpressionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManyToOneDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="manyToOneExpressionBuilder">The <see cref="IManyToOneExpressionBuilder"/> instance that 
        /// is responsible for creating an <see cref="Expression{TDelegate}"/> that maps many to one relations.</param>
        public ManyToOneDelegateBuilder(IManyToOneExpressionBuilder manyToOneExpressionBuilder)
        {
            this.manyToOneExpressionBuilder = manyToOneExpressionBuilder;
        }

        /// <summary>
        /// Creates a delegate that maps many to one relations.
        /// </summary>        
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate used to map many to one relations.</returns>
        public Action<IDataRecord, T> CreateDelegate(IDataRecord dataRecord)
        {
            Expression<Action<IDataRecord, T>> expression = manyToOneExpressionBuilder.CreateExpression<T>(dataRecord);
            return expression == null ? null : expression.Compile();
        }
    }
}