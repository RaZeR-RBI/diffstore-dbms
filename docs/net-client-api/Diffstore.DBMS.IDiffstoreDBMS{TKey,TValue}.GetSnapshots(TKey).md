# GetSnapshots(TKey)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Fetches snapshots for the specified entity key.


## Syntax

```csharp
public Task&lt;IEnumerable&lt;Snapshot&lt;TKey, TValue&gt;&gt;&gt; GetSnapshots(
	TKey key
)
```

------

[Back to index](index.md)