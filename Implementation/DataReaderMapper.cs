namespace DbExtensions.Implementation
{
    using System.Collections.Generic;
    using System.Data;

    using DbExtensions.Interfaces;

    public class DataReaderMapper<T> : IDataReaderMapper<T> where T:class
    {
        private readonly IDataRecordMapper<T> dataRecordMapper;

        public DataReaderMapper(IDataRecordMapper<T> dataRecordMapper)
        {
            this.dataRecordMapper = dataRecordMapper;
        }

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