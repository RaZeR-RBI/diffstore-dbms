# GetLast(TKey)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS<TKey, TValue>](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Returns the last snapshot of the specified entity.


## Syntax

```csharp
public Task<Snapshot<TKey, TValue>> GetLast(
	TKey key
)
```

------

[Back to index](index.md)