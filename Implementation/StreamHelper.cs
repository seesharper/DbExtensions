namespace DbExtensions.Implementation
{
    using System.Data;

    /// <summary>
    /// A helper class used to read streams from an <see cref="IDataRecord"/> instance.
    /// </summary>
    public class StreamHelper
    {
        /// <summary>
        /// Reads a byte array from the current <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The ordinal for the field that contains the stream data.</param>
        /// <returns>A new byte array.</returns>
        public static byte[] ReadByteArray(IDataRecord dataRecord, int ordinal)
        {
            long length = dataRecord.GetBytes(ordinal, 0, null, 0, int.MaxValue);
            var buffer = new byte[length];
            dataRecord.GetBytes(ordinal, 0, buffer, 0, (int)length);
            return buffer;
        }

        /// <summary>
        /// Reads a char array from the current <see cref="IDataRecord"/>.
        /// </summary>
        /// <param name="dataRecord">The target <see cref="IDataRecord"/>.</param>
        /// <param name="ordinal">The ordinal for the field that contains the stream data.</param>
        /// <returns>A new char array.</returns>
        public static char[] ReadCharArray(IDataRecord dataRecord, int ordinal)
        {
            long length = dataRecord.GetChars(ordinal, 0, null, 0, int.MaxValue);
            var buffer = new char[length];
            dataRecord.GetChars(ordinal, 0, buffer, 0, (int)length);
            return buffer;
        }
    }
}