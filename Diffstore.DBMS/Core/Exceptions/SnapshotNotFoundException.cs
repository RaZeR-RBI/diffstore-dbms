using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    /// <summary>
    /// Thrown if the specified entity has no snapshots.
    /// </summary>
    public class SnapshotNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SnapshotNotFoundException() : base("No snapshots found for the specified key") { }

        /// <summary>
        /// Includes key in the message.
        /// </summary>
        public SnapshotNotFoundException(object key) : base($"No snapshots found for the key {key}") { }

        /// <summary>
        /// Includes key in the message and wraps existing exception.
        /// </summary>
        public SnapshotNotFoundException(object key, Exception innerException) :
            base($"No snapshots found for the key {key}", innerException)
        { }
    }
}