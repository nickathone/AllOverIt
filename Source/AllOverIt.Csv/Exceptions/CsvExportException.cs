using System;

namespace AllOverIt.Csv.Exceptions
{
    /// <summary>Represents an error that occurs during the export of data to CSV.</summary>
    public sealed class CsvExportException : Exception
    {
        /// <summary>Default constructor.</summary>
        public CsvExportException()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        public CsvExportException(string message)
            : base(message)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CsvExportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}