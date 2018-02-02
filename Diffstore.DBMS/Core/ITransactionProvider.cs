using System;
using System.Collections.Generic;

namespace Diffstore.DBMS.Core
{
    public interface ITransactionProvider<TKey>
        where TKey : IComparable
    {
        bool BeginRead(TKey key);
        bool EndRead(TKey key);
        bool BeginWrite(TKey key);
        bool EndWrite(TKey key);
        IEnumerable<TKey> InRead { get; }
        IEnumerable<TKey> InWrite { get; }
    }

    public static class TransactionProvider
    {
        public static ITransactionProvider<TKey> OfType<TKey>() where TKey : IComparable =>
            new ConcurrentTransactionProvider<TKey>();
    }
}