# GetSnapshotsBetween(TKey, long, long)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.RemoteDBMS<TKey, TValue>](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)

------



Fetches snapshots in time range [timeStart, timeEnd).


## Syntax

```csharp
public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(
	TKey key,
	long timeStart,
	long timeEnd
)
```

------

[Back to index](index.md)