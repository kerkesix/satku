namespace KsxEventTracker.Domain.Aggregates.Checkpoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public class CheckpointState : AggregateRootStateBase
    {
        public CheckpointState()
        {
            this.ReadingsIn = new List<CheckpointScan>();
            this.ReadingsOut = new List<CheckpointScan>();
        }

        public string Id { get; set; }

        public int Order { get; set; }

        public CheckpointType CheckpointType { get; set; }

        public string Name { get; set; }

        public decimal DistanceFromPrevious { get; set; }

        public decimal DistanceFromStart { get; set; }
        public List<CheckpointScan> ReadingsIn { get; private set; }
        public List<CheckpointScan> ReadingsOut { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public void When(CheckpointCreated e)
        {
            this.Id = e.Id;
            this.Order = e.Order;
            this.CheckpointType = e.CheckpointType;
            this.Name = e.Name;
            this.Latitude = e.Latitude;
            this.Longitude = e.Longitude;
            this.DistanceFromPrevious = e.DistanceFromPrevious;
            this.DistanceFromStart = e.DistanceFromStart;
        }

        public void When(AttendeeScanIn e)
        {
            this.ReadingsIn.Add(new CheckpointScan(e));
        }

        public void When(AttendeeScanOut e)
        {
            this.ReadingsOut.Add(new CheckpointScan(e));
        }

        public void When(AttendeePassthroughScan e)
        {
            // Add same time to in and out
            this.ReadingsIn.Add(new CheckpointScan(e));
            this.ReadingsOut.Add(new CheckpointScan(e));
        }

        public void When(AttendeeScanInTimeChanged e)
        {
            this.ChangeScanInTime(e);
        }

        public void When(AttendeeScanOutTimeChanged e)
        {
            this.ChangeScanOutTime(e);
        }

        public void When(AttendeePassthroughScanTimeChanged e)
        {
            this.ChangeScanInTime(e);
            this.ChangeScanOutTime(e);
        }

        public void When(AttendeeDoubleScan e) { }

        public void When(AttendeeScanOutPreceedsScanIn e) { }

        public void When(AttendeeScanInRemoved e)
        {
            this.ReadingsIn.RemoveAll(r => r.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
        }

        public void When(AttendeeScanOutRemoved e)
        {
            this.ReadingsOut.RemoveAll(r => r.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
        }

        public void When(AttendeeScannedAtFinishCheckpoint e)
        {
        }

        public void When(AttendeeScannedAtStartCheckpoint e)
        {
        }

        public void When(FirstAttendeeForHappeningScanned e) { }

        private void ChangeScanInTime(ChangeScanTimeBase e)
        {
            var readingIn =
                this.ReadingsIn.First(m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
            readingIn.TimeStamp = e.NewTime;
        }

        private void ChangeScanOutTime(ChangeScanTimeBase e)
        {
            var readingOut =
                this.ReadingsOut.First(m => m.PersonId.Equals(e.PersonId, StringComparison.OrdinalIgnoreCase));
            readingOut.TimeStamp = e.NewTime;
        }
    }

    public class CheckpointScan
    {
        public string PersonId { get; private set; }

        public string ScanId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
        
        public CheckpointScan(AttendeeScanBase e)
        {
            this.PersonId = e.PersonId;
            this.TimeStamp = e.Timestamp;
            this.ScanId = e.ScanId;
        }
    }
}