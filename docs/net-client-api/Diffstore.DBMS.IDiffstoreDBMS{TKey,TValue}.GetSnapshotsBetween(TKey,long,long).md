# GetSnapshotsBetween(TKey, long, long)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Fetches snapshots in time range [timeStart, timeEnd).


## Syntax

```csharp
public Task&lt;IEnumerable&lt;Snapshot&lt;TKey, TValue&gt;&gt;&gt; GetSnapshotsBetween(
	TKey key,
	long timeStart,
	long timeEnd
)
```

------

[Back to index](index.md)