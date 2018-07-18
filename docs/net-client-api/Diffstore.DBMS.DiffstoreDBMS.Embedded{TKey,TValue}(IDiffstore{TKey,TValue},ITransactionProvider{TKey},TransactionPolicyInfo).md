# Embedded<TKey, TValue>(IDiffstore<TKey, TValue>, ITransactionProvider<TKey>, TransactionPolicyInfo)

**Method**

**Namespace:** [Diffstore.DBMS](Diffstore.DBMS.md)

**Declared in:** [Diffstore.DBMS.DiffstoreDBMS](Diffstore.DBMS.DiffstoreDBMS.md)

------



Creates an embedded (local storage) Diffstore DBMS instance.


## Syntax

```csharp
public static IDiffstoreDBMS<TKey, TValue> Embedded<TKey, TValue>(
	IDiffstore<TKey, TValue> db,
	ITransactionProvider<TKey> provider,
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