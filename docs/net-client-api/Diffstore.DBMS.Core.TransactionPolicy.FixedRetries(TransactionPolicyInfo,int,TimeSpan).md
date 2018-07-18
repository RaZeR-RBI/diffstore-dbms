# FixedRetries(TransactionPolicyInfo, int, TimeSpan)

**Method**

**Namespace:** [Diffstore.DBMS.Core](Diffstore.DBMS.Core.md)

**Declared in:** [Diffstore.DBMS.Core.TransactionPolicy](Diffstore.DBMS.Core.TransactionPolicy.md)

------



Creates a [TransactionPolicyInfo](Diffstore.DBMS.Core.TransactionPolicyInfo.md) with the specified
retry count and wait time.


## Syntax

```csharp
public static TransactionPolicyInfo FixedRetries(
	TransactionPolicyInfo policy,
	int retries,
	TimeSpan timeBetweenRetries
)
```

------

[Back to index](index.md)