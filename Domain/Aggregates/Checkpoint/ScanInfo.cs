namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    using System;

    public class ScanInfo
    {
        public ScanInfo(
            string happeningId, 
            string checkpointId, 
            string personId, 
            string scanId, 
            DateTimeOffset timestamp, 
            DateTimeOffset scanTimestamp,
            string readBy)
        {
            this.HappeningId = happeningId;
            this.CheckpointId = checkpointId;
            this.PersonId = personId;
            this.ScanId = scanId;
            this.Timestamp = timestamp;
            this.ScanTimestamp = scanTimestamp;
            this.ReadBy = readBy;
        }

        public string HappeningId { get; private set; }

        public string CheckpointId { get; private set; }
    
        public string PersonId { get; private set; }

        public string ScanId { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }
        public DateTimeOffset ScanTimestamp { get; set; }

        public string ReadBy { get; private set; }
    }
}