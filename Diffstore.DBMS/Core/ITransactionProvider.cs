using System;
using System.Collections.Generic;

namespace Diffstore.DBMS.Core
{
    public interface ITransactionProvider<TKey>
        where TKey : IComparable
    {
        bool Begin(TKey key);
        bool End(TKey key);
        bool IsAvailable(TKey key);
        IEnumerable<TKey> InProgress { get; }
    }
}