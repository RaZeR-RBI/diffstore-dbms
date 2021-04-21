# EmbeddedDBMS(IDiffstore&lt;TKey, TValue&gt;, TransactionPolicyInfo, ITransactionProvider&lt;TKey&gt;)

**Constructor**

**Namespace:** [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)

**Declared in:** [Diffstore.DBMS.Drivers.EmbeddedDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.md)

------



Default constructor.


## Syntax

```csharp
public EmbeddedDBMS(
	IDiffstore&lt;TKey, TValue&gt; db,
	TransactionPolicyInfo policy,
	ITransactionProvider&lt;TKey&gt; transaction
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