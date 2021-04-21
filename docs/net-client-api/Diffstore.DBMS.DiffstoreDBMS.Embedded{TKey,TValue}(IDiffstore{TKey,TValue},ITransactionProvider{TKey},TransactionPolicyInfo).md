# Embedded&lt;TKey, TValue&gt;(IDiffstore&lt;TKey, TValue&gt;, ITransactionProvider&lt;TKey&gt;, TransactionPolicyInfo)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.DiffstoreDBMS](Diffstore.DBMS.DiffstoreDBMS.md)

------



Creates an embedded (local storage) Diffstore DBMS instance.


## Syntax

```csharp
public static IDiffstoreDBMS&lt;TKey, TValue&gt; Embedded&lt;TKey, TValue&gt;(
	IDiffstore&lt;TKey, TValue&gt; db,
	ITransactionProvider&lt;TKey&gt; provider,
	TransactionPolicyInfo policy
)
```

### Parameters

`db`

Existing Diffstore instance

`provider`

ITransactionProvider

`policy`

Transaction policy options

------

[Back to index](index.md)