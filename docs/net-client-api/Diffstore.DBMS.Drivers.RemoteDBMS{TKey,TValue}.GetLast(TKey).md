# GetLast(TKey)

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.RemoteDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)

------



Returns the last snapshot of the specified entity.


## Syntax

```csharp
public async Task&lt;Snapshot&lt;TKey, TValue&gt;&gt; GetLast(
	TKey key
)
```

------

[Back to index](index.md)