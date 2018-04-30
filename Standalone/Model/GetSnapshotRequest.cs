namespace Standalone.Model
{
    public class GetSnapshotRequest
    {
        public int? From { get; set; }
        public int? Count { get; set; }
        public long? TimeStart { get; set; }
        public long? TimeEnd { get; set; }

        public bool IsPage() => From.HasValue && Count.HasValue;
        public bool IsTimeframe() => TimeStart.HasValue && TimeEnd.HasValue;
    }
}