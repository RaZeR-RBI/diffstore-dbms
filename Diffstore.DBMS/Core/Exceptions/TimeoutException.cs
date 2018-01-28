using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class TimeoutException : Exception
    {
        public TimeoutException() : base() { }
        public TimeoutException(string message) : base(message) { }
        public TimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}