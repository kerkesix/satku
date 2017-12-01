namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    public class AttendeeExtended<T>
    {
        public T Entity { get; set; }

        public Guid AttendeeId { get; set; }
    }
}