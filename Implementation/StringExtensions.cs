namespace DbExtensions.Implementation
{
    /// <summary>
    /// Extends the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Applies the given <paramref name="arguments"/> and returns the formatted string.
        /// </summary>
        /// <param name="value">The target <see cref="string"/> value.</param>
        /// <param name="arguments">An object array that contains zero or more arguments used to format the <paramref name="value"/> </param>
        /// <returns>A copy of the <paramref name="value"/> in which the format items have been replaced by the string representation of the <paramref name="arguments"/>.</returns>
        public static string FormatWith(this string value, params object[] arguments)
        {
            return string.Format(value, arguments);
        }
    }
}