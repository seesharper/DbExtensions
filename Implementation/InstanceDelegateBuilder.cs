namespace DbExtensions.Implementation
{
    using System;
    using System.Data;
    using System.Linq.Expressions;

    using DbExtensions.Interfaces;

    /// <summary>
    /// A class that is capable of creating a delegate that 
    /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the delegate.</typeparam>
    public class InstanceDelegateBuilder<T> : IInstanceDelegateBuilder<T> where T : class
    {
        private readonly IInstanceExpressionBuilder<T> instanceExpressionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceDelegateBuilder{T}"/> class.
        /// </summary>
        /// <param name="instanceExpressionBuilder">
        /// The <see cref="IInstanceExpressionBuilder{T}"/> that is responsible for creating an <see cref="Expression{TDelegate}"/> 
        /// that represents creating an instance of <typeparamref name="T"/>.
        /// </param>
        public InstanceDelegateBuilder(IInstanceExpressionBuilder<T> instanceExpressionBuilder)
        {
            this.instanceExpressionBuilder = instanceExpressionBuilder;            
        }

        /// <summary>
        /// Creates a delegate that 
        /// produces an instance of <typeparamref name="T"/> based an a <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <returns>A delegate that represents creating an instance of <typeparamref name="T"/>.</returns>
        public Func<IDataRecord, T> CreateInstanceDelegate(IDataRecord dataRecord)
        {
            var expression = instanceExpressionBuilder.CreateExpression(dataRecord);
            return expression.Compile();
        }        
    }
}