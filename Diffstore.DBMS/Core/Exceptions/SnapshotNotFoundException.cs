using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class SnapshotNotFoundException : Exception
    {
        public SnapshotNotFoundException() : base("No snapshots found for the specified key") { }

        public SnapshotNotFoundException(object key) : base($"No snapshots found for the key {key}") { }

        public SnapshotNotFoundException(object key, Exception innerException) : 
            base($"No snapshots found for the key {key}", innerException) { }
    }
}