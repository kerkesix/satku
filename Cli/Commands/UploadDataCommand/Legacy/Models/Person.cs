namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;

    /// <summary>
    /// Person accross all happenings. 
    /// </summary>
    public class Person
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string TwitterName { get; set; }

        public bool MayShowNameToPublic { get; set; }

        public int HappeningsAttended { get; set; }

        public int HappeningsCompleted { get; set; }

        public bool QuitLastTime { get; set; }

        public string NewEventBasedId { get; set; }
    }
}