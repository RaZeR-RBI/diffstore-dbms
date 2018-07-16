# GetSnapshotsBetween(TKey, long, long)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS<TKey, TValue>](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------


*No description provided*

## Syntax

```csharp
public Task<IEnumerable<Snapshot<TKey, TValue>>> GetSnapshotsBetween(
	TKey key,
	long timeStart,
	long timeEnd
)
```

------

[Back to index](index.md)