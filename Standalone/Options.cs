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
        public StorageMethod Store { get; set; }
        public FileFormat EntityFormat { get; set; }
        public FileFormat SnapshotFormat { get; set; }
        public SnapshotStorage Snapshots { get; set; }
        public bool LoadSchemaFromStdIn { get; set; }
    }
}