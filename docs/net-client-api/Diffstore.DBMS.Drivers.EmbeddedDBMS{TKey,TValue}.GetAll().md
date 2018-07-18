# GetAll()

**Method**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.EmbeddedDBMS<TKey, TValue>](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.md)

------



Returns all saved entities.


## Syntax

```csharp
public async Task<IEnumerable<Entity<TKey, TValue>>> GetAll()
```

## Remarks
This call may be slow depending on number of entities
------

[Back to index](index.md)