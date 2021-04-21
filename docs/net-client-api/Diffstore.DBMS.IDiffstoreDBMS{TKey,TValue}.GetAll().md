# GetAll()

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.IDiffstoreDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)

------



Returns all saved entities.


## Syntax

```csharp
public Task&lt;IEnumerable&lt;Entity&lt;TKey, TValue&gt;&gt;&gt; GetAll()
```

## Remarks
This call may be slow depending on number of entities
------

[Back to index](index.md)