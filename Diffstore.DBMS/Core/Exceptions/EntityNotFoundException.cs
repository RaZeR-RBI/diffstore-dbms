using System;

/// <summary>
/// Defines exceptions used by this library.
/// </summary>
namespace Diffstore.DBMS.Core.Exceptions
{
    /// <summary>
    /// Thrown when an entity with the specified key is not found.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EntityNotFoundException() : base("Cannot find entity with the specified key") { }

        /// <summary>
        /// Constructs an exception with the specified key included in the message .
        /// </summary>
        public EntityNotFoundException(object key) : base($"Cannot find entity by key {key}") { }

        /// <summary>
        /// Constructs an exception with the specified key included in the message
        ///  and wrapping existing exception.
        /// </summary>
        public EntityNotFoundException(object key, Exception innerException) :
            base($"Cannot find entity by key {key}", innerException)
        { }
    }
}