namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;

    public class Reading
    {
        public Guid Id { get; set; }

        public Guid CheckpointId { get; set; }

        public DateTime Timestamp { get; set; }

        public ReadingType ReadingType { get; set; }

        public DateTime SavedAt { get; set; }

        public string SavedBy { get; set; }
    }
}