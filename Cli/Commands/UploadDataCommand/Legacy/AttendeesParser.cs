namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class AttendeesParser : ParserBase<Attendee>
    {
        public AttendeesParser(string csvRaw)
            : base(csvRaw, 4)
        {
        }

        public override Attendee ParseRow()
        {
            // Id;Barcode;PersonId;HappeningId
            return new Attendee
            {
                Id = this.Next<Guid>(),
                Barcode = this.Next(),
                PersonId = this.Next<Guid>(),
                HappeningId = this.Next()
            };
        }
    }
}