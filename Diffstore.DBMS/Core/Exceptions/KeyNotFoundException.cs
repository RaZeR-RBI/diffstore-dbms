using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException() : base("Cannot find entity with the specified key") { }

        public KeyNotFoundException(object key) : base($"Cannot find entity by key {key}") { }

        public KeyNotFoundException(object key, Exception innerException) : 
            base($"Cannot find entity by key {key}", innerException) { }
    }
}