# EmbeddedDBMS(IDiffstore<TKey, TValue>, TransactionPolicyInfo, ITransactionProvider<TKey>)

**Constructor**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.EmbeddedDBMS<TKey, TValue>](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.md)

------



Default constructor.


## Syntax

```csharp
public EmbeddedDBMS(
	IDiffstore<TKey, TValue> db,
	TransactionPolicyInfo policy,
	ITransactionProvider<TKey> transaction
)
```

### Parameters

`db`

Existing Diffstore instance

`policy`

Transaction policy options

`transaction`

Transaction provider instance

------

[Back to index](index.md)