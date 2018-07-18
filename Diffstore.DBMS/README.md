# How to use Diffstore DBMS .NET client
You can connect to remote DBMS instance using the following example code:
```csharp
using Diffstore.DBMS;

// uses default connection uri
var driver = DiffstoreDBMS.Remote<long, MyEntityType>();
// uses the specified connection uri
var driver = DiffstoreDBMS.Remote<long, MyEntityType>("www.example.com");
```

If you want to instantiate a local one, you can check out this example code:
```csharp
using Diffstore; // make sure you have Diffstore NuGet package installed
using Diffstore.DBMS;
using Diffstore.DBMS.Core;

var db = ...; // instantiate Diffstore instance, see Diffstore project

var driver = DiffstoreDBMS.Embedded<long, MyEntityType>(
    db, 
    TransactionPolicy.FixedRetries(3, TimeSpan.FromMilliseconds(1000)),
    new ConcurrentTransactionProvider<long>()
);
```