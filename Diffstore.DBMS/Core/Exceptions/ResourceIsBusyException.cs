using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class ResourceIsBusyException : Exception
    {
        public ResourceIsBusyException() : base() { }
        public ResourceIsBusyException(string message) : base(message) { }
        public ResourceIsBusyException(string message, Exception innerException) : base(message, innerException) { }
    }
}