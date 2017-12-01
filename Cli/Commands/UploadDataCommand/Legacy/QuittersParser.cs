namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class QuittersParser : ParserBase<AttendeeExtended<Quitter>>
    {
        public QuittersParser(string csvRaw)
            : base(csvRaw, 9)
        {
        }

        public override AttendeeExtended<Quitter> ParseRow()
        {
            // Id;CheckpointId;Timestamp;QuitReason;WalkedSinceLastCheckpoint;Description;SavedAt;SavedBy;Attendee_Id
            var q = new Quitter
            {
                Id = this.Next<Guid>(),
                CheckpointId = this.Next<Guid>(),
                Timestamp = this.Next<DateTime>(),
                QuitReason = this.Next<QuitReason>(),
                WalkedSinceLastCheckpoint = this.Next<decimal>(),
                Description = this.Next(),
                SavedAt = this.Next<DateTime>(),
                SavedBy = this.Next()
            };

            // Fix datetimes for 2012 event
            if (q.Timestamp.Year == 2012)
            {
                q.Timestamp = q.Timestamp.AddHours(3);
            }

            return new AttendeeExtended<Quitter> { Entity = q, AttendeeId = this.Next<Guid>() };
        }
    }
}