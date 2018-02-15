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
        public StorageMethod Store = StorageMethod.OnDisk;
        public FileFormat EntityFormat = FileFormat.JSON;
        public FileFormat SnapshotFormat = FileFormat.JSON;
        public SnapshotStorage Snapshots = SnapshotStorage.SingleFile;
        public bool LoadSchemaFromStdIn = false;
    }
}