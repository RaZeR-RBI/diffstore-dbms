using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    /// <summary>
    /// Thrown if the specified entity or snapshot is blocked by other action.
    /// </summary>
    public class ResourceIsBusyException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ResourceIsBusyException() : base() { }
        /// <summary>
        /// Includes a message in the exception.
        /// </summary>
        public ResourceIsBusyException(string message) : base(message) { }
        /// <summary>
        /// Includes a message and inner exception.
        /// </summary>
        public ResourceIsBusyException(string message, Exception innerException) : base(message, innerException) { }
    }
}