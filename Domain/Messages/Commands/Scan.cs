namespace KsxEventTracker.Domain.Messages.Commands
{
    using System;

    public class Scan: PersonAtCheckpointBase, ICommand
    {
        /// <summary>
        /// Should be used when time is not system created, but given by a user. 
        /// </summary>
        public DateTimeOffset? ScanTimestamp { get; set; }
    }
}