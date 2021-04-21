# GetLast(TKey)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Returns the last snapshot of the specified entity.


## Syntax

```csharp
public Task&lt;Snapshot&lt;TKey, TValue&gt;&gt; GetLast(
	TKey key
)
```

------

[Back to index](index.md)