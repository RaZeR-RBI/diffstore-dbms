[![Build status](https://ci.appveyor.com/api/projects/status/3g1irn5yu15i0334?svg=true)](https://ci.appveyor.com/project/RaZeR-RawByte/diffstore-dbms) [![GitHub license](https://img.shields.io/github/license/RaZeR-RBI/diffstore-dbms.svg)](https://github.com/RaZeR-RBI/diffstore-dbms/blob/master/LICENSE) 

---

# Diffstore DBMS
This project is a DMBS-like REST interface for the [Diffstore](https://github.com/razer-rbi/diffstore) storage engine.

# Installation
## Standalone server
[![NuGet Version](https://img.shields.io/nuget/v/Diffstore.DBMS.svg)](https://www.nuget.org/packages/Diffstore.DBMS) [![NuGet](https://img.shields.io/nuget/dt/Diffstore.DBMS.svg)](https://www.nuget.org/packages/Diffstore.DBMS)
```
dotnet tool install -g Diffstore.DBMS
```

## Clients
### C# client
[![NuGet Version](https://img.shields.io/nuget/v/Diffstore.DBMS.Client.svg)](https://www.nuget.org/packages/Diffstore.DBMS.Client) [![NuGet](https://img.shields.io/nuget/dt/Diffstore.DBMS.Client.svg)](https://www.nuget.org/packages/Diffstore.DBMS.Client)

```
dotnet add package Diffstore.DBMS.Client
```

# Server options
Run the following command to get help:
```diffstore-dbms -?```

Command line parameters:

* **store** - Storage location. Values: OnDisk, InMemory. Default:OnDisk
* **entityFormat** - Entity storage format. Values: JSON, XML,Binary. Default: JSON
* **snapshotFormat** - Snapshot storage format. Values: JSON, XML,Binary. Default: JSON
* **snapshots** - Snapshot storage mechanism. Values: SingleFile,LastFirst. Default: SingleFile
* **loadSchemaFromStdIn** - Determines if schema definition should beloaded from stdin. Default: false
* **keyType** - Entity key type. Default: long
* **listen** - One or several URIs to listen on. Default: http://localhost:8008
* **maxRetries** - Maximum retries if the requested entity is busy. Default: 5
* **retryTimeout** - Timeout in ms between retries if the requested entity is busy. Default: 1000

Sample schema can be found in REST API documentation (index path), or in the **E2E/schema.json** file.

# API Documentation
## REST API
[Server REST API Definition](https://razer-rbi.github.io/diffstore-dbms/rest-api/)

## Client implementations
[C# Client API](docs/net-client-api/index.md)

