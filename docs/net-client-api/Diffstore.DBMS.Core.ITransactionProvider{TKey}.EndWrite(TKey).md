# EndWrite(TKey)

**Method**

**Namespace:** [Diffstore.DBMS.Core](Diffstore.DBMS.Core.md)

**Declared in:** [Diffstore.DBMS.Core.ITransactionProvider<TKey>](Diffstore.DBMS.Core.ITransactionProvider{TKey}.md)

------



Removes write lock from the specified entity key.


## Syntax

```csharp
public bool EndWrite(
	TKey key
)
```

### Returns

True if unlocking was successful

------

[Back to index](index.md)