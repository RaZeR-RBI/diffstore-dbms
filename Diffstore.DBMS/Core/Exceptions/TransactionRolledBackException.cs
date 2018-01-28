using System;

namespace Diffstore.DBMS.Core.Exceptions
{
    public class TransactionRolledBackException : Exception
    {
        public TransactionRolledBackException() : base("Unable to finish the transaction, rolled back") { }

        public TransactionRolledBackException(object key) : base($"Unable to finish the transaction, rolled back {key}") { }

        public TransactionRolledBackException(object key, Exception innerException) : 
            base($"Unable to finish the transaction, rolled back {key}", innerException) { }
    }
}