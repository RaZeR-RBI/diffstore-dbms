# GetSnapshots(TKey)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.RemoteDBMS<TKey, TValue>](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)

------



Fetches snapshots for the specified entity key.


## Syntax

```csharp
public async Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshots(
	TKey key
)
```

------

[Back to index](index.md)