namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;

    public class Quitter
    {
        public Guid Id { get; set; }

        public Guid CheckpointId { get; set; }

        public virtual Checkpoint Checkpoint { get; set; }

        public virtual Attendee Attendee { get; set; }
        
        public DateTime Timestamp { get; set; }

        public QuitReason QuitReason { get; set; }

        public decimal WalkedSinceLastCheckpoint { get; set; }

        public string Description { get; set; }

        public DateTime SavedAt { get; set; }

        public string SavedBy { get; set; }
    }
}