using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base("Cannot find entity with the specified key") { }

        public EntityNotFoundException(object key) : base($"Cannot find entity by key {key}") { }

        public EntityNotFoundException(object key, Exception innerException) : 
            base($"Cannot find entity by key {key}", innerException) { }
    }
}