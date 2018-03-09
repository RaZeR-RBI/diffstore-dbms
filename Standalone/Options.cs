using System.Collections.Generic;
using Diffstore;

namespace Standalone
{
    public enum StorageMethod
    {
        InMemory, OnDisk
    }

    public enum SnapshotStorage
    {
        SingleFile, LastFirst
    }

    public class Options
    {
        // Storage-related
        public StorageMethod Store { get; set; }
        public FileFormat EntityFormat { get; set; }
        public FileFormat SnapshotFormat { get; set; }
        public SnapshotStorage Snapshots { get; set; }
        public bool LoadSchemaFromStdIn { get; set; }
        public string KeyType { get; set; }
        public int MaxRetries { get; set; }
        public int RetryTimeout { get; set; }

        // Frontend-related
        public List<string> Listeners { get; set; }
    }
}