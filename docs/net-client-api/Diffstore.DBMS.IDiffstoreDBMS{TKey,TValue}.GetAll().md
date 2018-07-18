# GetAll()

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS<TKey, TValue>](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Returns all saved entities.


## Syntax

```csharp
public Task<IEnumerable<Entity<TKey, TValue>>> GetAll()
```

## Remarks
This call may be slow depending on number of entities
------

[Back to index](index.md)