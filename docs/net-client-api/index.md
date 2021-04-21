# How to use Diffstore DBMS .NET client
You can connect to remote DBMS instance using the following example code:
```csharp
using Diffstore.DBMS;

// uses default connection uri
var driver = DiffstoreDBMS.Remote&lt;long, MyEntityType&gt;();
// uses the specified connection uri
var driver = DiffstoreDBMS.Remote&lt;long, MyEntityType&gt;("www.example.com");
```

If you want to instantiate a local one, you can check out this example code:
```csharp
using Diffstore; // make sure you have Diffstore NuGet package installed
using Diffstore.DBMS;
using Diffstore.DBMS.Core;

var db = ...; // instantiate Diffstore instance, see Diffstore project

var driver = DiffstoreDBMS.Embedded&lt;long, MyEntityType&gt;(
    db, 
    TransactionPolicy.FixedRetries(3, TimeSpan.FromMilliseconds(1000)),
    new ConcurrentTransactionProvider&lt;long&gt;()
);
```

# API Documentation

* [Diffstore.DBMS](Diffstore.DBMS.md)
    * [DiffstoreDBMS](Diffstore.DBMS.DiffstoreDBMS.md)
        * [Embedded&lt;TKey, TValue&gt;(IDiffstore&lt;TKey, TValue&gt;, ITransactionProvider&lt;TKey&gt;, TransactionPolicyInfo)](Diffstore.DBMS.DiffstoreDBMS.Embedded{TKey,TValue}(IDiffstore{TKey,TValue},ITransactionProvider{TKey},TransactionPolicyInfo).md)
        * [Remote&lt;TKey, TValue&gt;()](Diffstore.DBMS.DiffstoreDBMS.Remote{TKey,TValue}().md)
        * [Remote&lt;TKey, TValue&gt;(Uri)](Diffstore.DBMS.DiffstoreDBMS.Remote{TKey,TValue}(Uri).md)
    * [IDiffstoreDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.md)
        * [Delete(Entity&lt;TKey, TValue&gt;)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Delete(Entity{TKey,TValue}).md)
        * [Delete(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Delete(TKey).md)
        * [Exists(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Exists(TKey).md)
        * [Get(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Get(TKey).md)
        * [GetAll()](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetAll().md)
        * [GetFirst(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetFirst(TKey).md)
        * [GetFirstTime(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetFirstTime(TKey).md)
        * [GetLast(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetLast(TKey).md)
        * [GetLastTime(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetLastTime(TKey).md)
        * [GetSnapshots(TKey, int, int)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetSnapshots(TKey,int,int).md)
        * [GetSnapshots(TKey)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetSnapshots(TKey).md)
        * [GetSnapshotsBetween(TKey, long, long)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.GetSnapshotsBetween(TKey,long,long).md)
        * [Keys()](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Keys().md)
        * [PutSnapshot(Entity&lt;TKey, TValue&gt;, long)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.PutSnapshot(Entity{TKey,TValue},long).md)
        * [Save(Entity&lt;TKey, TValue&gt;, bool)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Save(Entity{TKey,TValue},bool).md)
        * [Save(TKey, TValue, bool)](Diffstore.DBMS.IDiffstoreDBMS{TKey,TValue}.Save(TKey,TValue,bool).md)


* [Diffstore.DBMS.Core](Diffstore.DBMS.Core.md)
    * [ConcurrentTransactionProvider&lt;TKey&gt;](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.md)
        * [InRead](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.InRead.md)
        * [InWrite](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.InWrite.md)
        * [BeginRead(TKey)](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.BeginRead(TKey).md)
        * [BeginWrite(TKey)](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.BeginWrite(TKey).md)
        * [EndRead(TKey)](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.EndRead(TKey).md)
        * [EndWrite(TKey)](Diffstore.DBMS.Core.ConcurrentTransactionProvider{TKey}.EndWrite(TKey).md)
    * [EntityExt&lt;TKey, TValue&gt;](Diffstore.DBMS.Core.EntityExt{TKey,TValue}.md)
        * [EntityExt()](Diffstore.DBMS.Core.EntityExt{TKey,TValue}.EntityExt().md)
        * [Create()](Diffstore.DBMS.Core.EntityExt{TKey,TValue}.Create().md)
    * [SnapshotExt&lt;TKey, TValue&gt;](Diffstore.DBMS.Core.SnapshotExt{TKey,TValue}.md)
        * [Time](Diffstore.DBMS.Core.SnapshotExt{TKey,TValue}.Time.md)
        * [State](Diffstore.DBMS.Core.SnapshotExt{TKey,TValue}.State.md)
        * [SnapshotExt()](Diffstore.DBMS.Core.SnapshotExt{TKey,TValue}.SnapshotExt().md)
        * [Create()](Diffstore.DBMS.Core.SnapshotExt{TKey,TValue}.Create().md)
    * [TransactionPolicy](Diffstore.DBMS.Core.TransactionPolicy.md)
        * [FixedRetries(int, TimeSpan)](Diffstore.DBMS.Core.TransactionPolicy.FixedRetries(int,TimeSpan).md)
        * [FixedRetries(TransactionPolicyInfo, int, TimeSpan)](Diffstore.DBMS.Core.TransactionPolicy.FixedRetries(TransactionPolicyInfo,int,TimeSpan).md)
        * [SingleRetry(TimeSpan)](Diffstore.DBMS.Core.TransactionPolicy.SingleRetry(TimeSpan).md)
        * [SingleRetry(TransactionPolicyInfo, TimeSpan)](Diffstore.DBMS.Core.TransactionPolicy.SingleRetry(TransactionPolicyInfo,TimeSpan).md)
        * [WithRetries(TimeSpan[])](Diffstore.DBMS.Core.TransactionPolicy.WithRetries(TimeSpan[]).md)
        * [WithRetries(TransactionPolicyInfo, TimeSpan[])](Diffstore.DBMS.Core.TransactionPolicy.WithRetries(TransactionPolicyInfo,TimeSpan[]).md)
    * [TransactionPolicyInfo](Diffstore.DBMS.Core.TransactionPolicyInfo.md)
        * [RetryTimeouts](Diffstore.DBMS.Core.TransactionPolicyInfo.RetryTimeouts.md)
    * [TransactionProvider](Diffstore.DBMS.Core.TransactionProvider.md)
        * [OfType&lt;TKey&gt;()](Diffstore.DBMS.Core.TransactionProvider.OfType{TKey}().md)
    * [ITransactionProvider&lt;TKey&gt;](Diffstore.DBMS.Core.ITransactionProvider{TKey}.md)
        * [InRead](Diffstore.DBMS.Core.ITransactionProvider{TKey}.InRead.md)
        * [InWrite](Diffstore.DBMS.Core.ITransactionProvider{TKey}.InWrite.md)
        * [BeginRead(TKey)](Diffstore.DBMS.Core.ITransactionProvider{TKey}.BeginRead(TKey).md)
        * [BeginWrite(TKey)](Diffstore.DBMS.Core.ITransactionProvider{TKey}.BeginWrite(TKey).md)
        * [EndRead(TKey)](Diffstore.DBMS.Core.ITransactionProvider{TKey}.EndRead(TKey).md)
        * [EndWrite(TKey)](Diffstore.DBMS.Core.ITransactionProvider{TKey}.EndWrite(TKey).md)


* [Diffstore.DBMS.Core.Exceptions](Diffstore.DBMS.Core.Exceptions.md)
    * [EntityNotFoundException](Diffstore.DBMS.Core.Exceptions.EntityNotFoundException.md)
        * [EntityNotFoundException()](Diffstore.DBMS.Core.Exceptions.EntityNotFoundException.EntityNotFoundException().md)
        * [EntityNotFoundException(object)](Diffstore.DBMS.Core.Exceptions.EntityNotFoundException.EntityNotFoundException(object).md)
        * [EntityNotFoundException(object, Exception)](Diffstore.DBMS.Core.Exceptions.EntityNotFoundException.EntityNotFoundException(object,Exception).md)
    * [ResourceIsBusyException](Diffstore.DBMS.Core.Exceptions.ResourceIsBusyException.md)
        * [ResourceIsBusyException()](Diffstore.DBMS.Core.Exceptions.ResourceIsBusyException.ResourceIsBusyException().md)
        * [ResourceIsBusyException(string)](Diffstore.DBMS.Core.Exceptions.ResourceIsBusyException.ResourceIsBusyException(string).md)
        * [ResourceIsBusyException(string, Exception)](Diffstore.DBMS.Core.Exceptions.ResourceIsBusyException.ResourceIsBusyException(string,Exception).md)
    * [SnapshotNotFoundException](Diffstore.DBMS.Core.Exceptions.SnapshotNotFoundException.md)
        * [SnapshotNotFoundException()](Diffstore.DBMS.Core.Exceptions.SnapshotNotFoundException.SnapshotNotFoundException().md)
        * [SnapshotNotFoundException(object)](Diffstore.DBMS.Core.Exceptions.SnapshotNotFoundException.SnapshotNotFoundException(object).md)
        * [SnapshotNotFoundException(object, Exception)](Diffstore.DBMS.Core.Exceptions.SnapshotNotFoundException.SnapshotNotFoundException(object,Exception).md)


* [Diffstore.DBMS.Drivers](Diffstore.DBMS.Drivers.md)
    * [EmbeddedDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.md)
        * [EmbeddedDBMS(IDiffstore&lt;TKey, TValue&gt;, TransactionPolicyInfo, ITransactionProvider&lt;TKey&gt;)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.EmbeddedDBMS(IDiffstore{TKey,TValue},TransactionPolicyInfo,ITransactionProvider{TKey}).md)
        * [Delete(Entity&lt;TKey, TValue&gt;)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Delete(Entity{TKey,TValue}).md)
        * [Delete(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Delete(TKey).md)
        * [Dispose()](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Dispose().md)
        * [Exists(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Exists(TKey).md)
        * [Get(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Get(TKey).md)
        * [GetAll()](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetAll().md)
        * [GetFirst(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetFirst(TKey).md)
        * [GetFirstTime(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetFirstTime(TKey).md)
        * [GetLast(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetLast(TKey).md)
        * [GetLastTime(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetLastTime(TKey).md)
        * [GetSnapshots(TKey, int, int)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetSnapshots(TKey,int,int).md)
        * [GetSnapshots(TKey)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetSnapshots(TKey).md)
        * [GetSnapshotsBetween(TKey, long, long)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.GetSnapshotsBetween(TKey,long,long).md)
        * [Keys()](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Keys().md)
        * [PutSnapshot(Entity&lt;TKey, TValue&gt;, long)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.PutSnapshot(Entity{TKey,TValue},long).md)
        * [Save(Entity&lt;TKey, TValue&gt;, bool)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Save(Entity{TKey,TValue},bool).md)
        * [Save(TKey, TValue, bool)](Diffstore.DBMS.Drivers.EmbeddedDBMS{TKey,TValue}.Save(TKey,TValue,bool).md)
    * [RemoteDBMS&lt;TKey, TValue&gt;](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.md)
        * [RemoteDBMS(Uri)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.RemoteDBMS(Uri).md)
        * [Delete(Entity&lt;TKey, TValue&gt;)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Delete(Entity{TKey,TValue}).md)
        * [Delete(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Delete(TKey).md)
        * [Dispose()](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Dispose().md)
        * [Exists(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Exists(TKey).md)
        * [Get(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Get(TKey).md)
        * [GetAll()](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetAll().md)
        * [GetFirst(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetFirst(TKey).md)
        * [GetFirstTime(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetFirstTime(TKey).md)
        * [GetLast(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetLast(TKey).md)
        * [GetLastTime(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetLastTime(TKey).md)
        * [GetSnapshots(TKey, int, int)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetSnapshots(TKey,int,int).md)
        * [GetSnapshots(TKey)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetSnapshots(TKey).md)
        * [GetSnapshotsBetween(TKey, long, long)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.GetSnapshotsBetween(TKey,long,long).md)
        * [Keys()](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Keys().md)
        * [PutSnapshot(Entity&lt;TKey, TValue&gt;, long)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.PutSnapshot(Entity{TKey,TValue},long).md)
        * [Save(Entity&lt;TKey, TValue&gt;, bool)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Save(Entity{TKey,TValue},bool).md)
        * [Save(TKey, TValue, bool)](Diffstore.DBMS.Drivers.RemoteDBMS{TKey,TValue}.Save(TKey,TValue,bool).md)


------

[Generated with DotBook](https://github.com/RaZeR-RBI/dotbook)