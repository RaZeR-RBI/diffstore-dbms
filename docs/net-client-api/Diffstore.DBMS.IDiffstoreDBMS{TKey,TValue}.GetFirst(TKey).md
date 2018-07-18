# GetFirst(TKey)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS<TKey, TValue>](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Returns the first snapshot of the specified entity.


## Syntax

```csharp
public Task<Snapshot<TKey, TValue>> GetFirst(
	TKey key
)
```

------

[Back to index](index.md)