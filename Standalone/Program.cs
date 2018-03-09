using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Diffstore;
using Fclp;
using Jil;
using Nancy;
using Nancy.Hosting.Self;
using Standalone.Core;
using Standalone.Nancy;

namespace Standalone
{

    class Program
    {
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser<Options>();
            p.Setup(x => x.Store)
                .As("store")
                .WithDescription(
                    "Storage location. Values: OnDisk, InMemory. Default: OnDisk"
                )
                .SetDefault(StorageMethod.OnDisk);

            p.Setup(x => x.EntityFormat)
                .As("entityFormat")
                .WithDescription(
                    "Entity storage format. Values: JSON, XML, Binary. Default: JSON"
                )
                .SetDefault(FileFormat.JSON);

            p.Setup(x => x.SnapshotFormat)
                .As("snapshotFormat")
                .WithDescription(
                    "Snapshot storage format. Values: JSON, XML, Binary. Default: JSON"
                )
                .SetDefault(FileFormat.JSON);

            p.Setup(x => x.Snapshots)
                .As("snapshots")
                .WithDescription(
                    "Snapshot storage mechanism. Values: SingleFile, LastFirstOptimized. " +
                    "Default: SingleFile"
                )
                .SetDefault(SnapshotStorage.SingleFile);

            p.Setup(x => x.LoadSchemaFromStdIn)
                .As("loadSchemaFromStdIn")
                .WithDescription(
                    "Determines if schema definition should be loaded from stdin. " +
                    "Default: false"
                )
                .SetDefault(false);

            p.Setup(x => x.KeyType)
                .As("keyType")
                .WithDescription(
                    "Entity key type. Default: long"
                )
                .SetDefault("long");

            p.Setup(x => x.Listeners)
                .As("port")
                .WithDescription(
                    "One or several URIs to listen on. Default: http://localhost:8008"
                )
                .SetDefault(new List<string> { "http://localhost:8008" });

            p.Setup(x => x.MaxRetries)
                .As("maxRetries")
                .WithDescription(
                    "Maximum retries if the requested entity is busy. " +
                    "Default: 5"
                )
                .SetDefault(5);

            p.Setup(x => x.RetryTimeout)
                .As("retryTimeout")
                .WithDescription(
                    "Timeout in ms between retries if the requested entity is busy. " +
                    "Default: 1000"
                )
               .SetDefault(1000);

            p.SetupHelp("?", "h", "help")
                .Callback(text =>
                {
                    Console.WriteLine(text);
                    Environment.Exit(0);
                });

            var result = p.Parse(args);
            if (!result.HasErrors)
                Run(p.Object);
            else
                foreach (var error in result.Errors)
                    Console.WriteLine($"[FATAL] {error}");
        }

        static void Run(Options options)
        {
            var URIs = options.Listeners.Select(s => new Uri(s)).ToArray();
            var schema = LoadSchema(options.LoadSchemaFromStdIn);
            var bootstrapper = new MainBootstrapper(options, schema);
            using (var host = new NancyHost(bootstrapper, URIs))
            {
                host.Start();
                Console.WriteLine(
                    $"Listening on {string.Join(", ", options.Listeners)}. " +
                    "Press any key to stop."
                );
                Console.ReadKey();
                Console.WriteLine("Stopping...");
                host.Stop();
            }
        }

        static SchemaDefinition LoadSchema(bool fromStdIn) =>
            JSON.Deserialize<SchemaDefinition>(fromStdIn ? 
                ReadStdIn() :
                File.ReadAllText("schema.json"));

        static string ReadStdIn()
        {
            string stdin = null;
            if (Console.IsInputRedirected)
            {
                using (var stream = Console.OpenStandardInput())
                {
                    var buffer = new byte[1000]; 
                    var builder = new StringBuilder();
                    var read = -1;
                    while (true)
                    {
                        var gotInput = new AutoResetEvent(false);
                        var inputThread = new Thread(() =>
                        {
                            try
                            {
                                read = stream.Read(buffer, 0, buffer.Length);
                                gotInput.Set();
                            }
                            catch (ThreadAbortException)
                            {
                                Thread.ResetAbort();
                            }
                        })
                        {
                            IsBackground = true
                        };
                        inputThread.Start();
                        if (!gotInput.WaitOne(100))
                        {
                            inputThread.Abort();
                            break;
                        }
                        if (read == 0)
                        {
                            stdin = builder.ToString();
                            break;
                        }
                        builder.Append(Console.InputEncoding.GetString(buffer, 0, read));
                    }
                }
            }
            return stdin;
        }
    }
}
