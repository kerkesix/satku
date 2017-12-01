namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models
{
    using System;

    /// <summary>
    /// Instance of a person that attends happening.
    /// </summary>
    public class Attendee
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the barcode.
        /// </summary>
        public string Barcode { get; set; }

        public Guid PersonId { get; set; }

        public string HappeningId { get; set; }
    }
}