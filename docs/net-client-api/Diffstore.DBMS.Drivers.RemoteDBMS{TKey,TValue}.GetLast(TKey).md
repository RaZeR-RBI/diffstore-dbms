# GetLast(TKey)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.RemoteDBMS<TKey, TValue>](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)

------



Returns the last snapshot of the specified entity.


## Syntax

```csharp
public async Task<Snapshot<TKey, TValue>> GetLast(
	TKey key
)
```

------

[Back to index](index.md)