# GetSnapshots(TKey, int, int)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.EmbeddedDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.md)

------



Fetches snapshots page for the specified entity key.


## Syntax

```csharp
public async Task&lt;IEnumerable&lt;Snapshot&lt;TKey, TValue&gt;&gt;&gt; GetSnapshots(
	TKey key,
	int from,
	int count
)
```

------

[Back to index](index.md)