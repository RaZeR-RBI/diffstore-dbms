# GetSnapshotsBetween(TKey, long, long)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.RemoteDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)

------



Fetches snapshots in time range [timeStart, timeEnd).


## Syntax

```csharp
public async Task&lt;IEnumerable&lt;Snapshot&lt;TKey, TValue&gt;&gt;&gt; GetSnapshotsBetween(
	TKey key,
	long timeStart,
	long timeEnd
)
```

------

[Back to index](index.md)