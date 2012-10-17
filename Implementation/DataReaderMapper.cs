namespace DbExtensions.Implementation
{
    using System.Collections.Generic;
    using System.Data;

    using DbExtensions.Interfaces;

    /// <summary>
    /// Maps a <see cref="IDataReader"/> into an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be returned from the mapper.</typeparam>
    public class DataReaderMapper<T> : IDataReaderMapper<T> where T : class
    {
        private readonly IDataRecordMapper<T> dataRecordMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReaderMapper{T}"/> class.
        /// </summary>
        /// <param name="dataRecordMapper">The <see cref="IDataRecordMapper{T}"/> that is responsible for 
        /// mapping a <see cref="IDataRecord"/> to an instance of <typeparamref name="T"/>.</param>
        public DataReaderMapper(IDataRecordMapper<T> dataRecordMapper)
        {
            this.dataRecordMapper = dataRecordMapper;
        }

        /// <summary>
        /// Executes the transformation.
        /// </summary>
        /// <param name="dataReader">The target <see cref="IDataReader"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> that represents the result of the mapping/transformation.</returns>
        public IEnumerable<T> Execute(IDataReader dataReader)
        {
            IList<T> result = new List<T>();            
            while (dataReader.Read())
            {
                var instance = dataRecordMapper.Execute(dataReader);
                if (!result.Contains(instance))
                {
                    result.Add(instance);
                }
            }

            return result;
        }
    }
}