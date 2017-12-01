namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class ReadingsParser : ParserBase<AttendeeExtended<Reading>>
    {
        public ReadingsParser(string csvRaw)
            : base(csvRaw, 7)
        {
        }

        public override AttendeeExtended<Reading> ParseRow()
        {
            // Id;CheckpointId;Timestamp;ReadingType;SavedAt;SavedBy;Attendee_Id
            var r = new Reading
            {
                Id = this.Next<Guid>(),
                CheckpointId = this.Next<Guid>(),
                Timestamp = this.Next<DateTime>(),
                ReadingType = this.Next<ReadingType>(),
                SavedAt = this.Next<DateTime>(),
                SavedBy = this.Next()
            };

            // Fix datetimes for 2012 event
            if (r.Timestamp.Year == 2012)
            {
                r.Timestamp = r.Timestamp.AddHours(3);
            }

            return new AttendeeExtended<Reading> { AttendeeId = this.Next<Guid>(), Entity = r };
        }
    }
}