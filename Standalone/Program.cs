using System;
using Diffstore;
using Fclp;

namespace Standalone
{

    class Program
    {
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser<Options>();
            p.Setup(x => x.Store)
                .As("store")
                .SetDefault(StorageMethod.OnDisk);

            p.Setup(x => x.EntityFormat)
                .As("entityFormat")
                .SetDefault(FileFormat.JSON);

            p.Setup(x => x.SnapshotFormat)
                .As("snapshotFormat")
                .SetDefault(FileFormat.JSON);

            p.Setup(x => x.Snapshots)
                .As("snapshots")
                .SetDefault(SnapshotStorage.SingleFile);

            p.Setup(x => x.LoadSchemaFromStdIn)
                .As("loadSchemaFromStdIn")
                .SetDefault(false);
            
            p.Setup(x => x.KeyType)
                .As("keyType")
                .SetDefault("long");

            p.Setup(x => x.Port)
                .As("port")
                .SetDefault(8008);

            var result = p.Parse(args);
            if (!result.HasErrors)
                Run(p.Object);
            else
                foreach (var error in result.Errors)
                    Console.WriteLine($"[FATAL] {error}");
        }

        static void Run(Options options)
        {
            // TODO
        }
    }
}
