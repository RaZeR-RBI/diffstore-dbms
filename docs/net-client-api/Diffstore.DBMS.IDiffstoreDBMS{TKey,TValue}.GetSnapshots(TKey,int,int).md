# GetSnapshots(TKey, int, int)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS<TKey, TValue>](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Fetches snapshots page for the specified entity key.


## Syntax

```csharp
public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(
	TKey key,
	int from,
	int count
)
```

------

[Back to index](index.md)