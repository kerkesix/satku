namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class HappeningsParser : ParserBase<Happening>
    {
        public HappeningsParser(string csvRaw)
            : base(csvRaw, 3)
        {
        }

        public override Happening ParseRow()
        {
            // Id;Default;CreatedAt
            return new Happening
            {   
                Id = this.Next(),
                Default = this.Next<bool>(),
                CreatedAt = this.Next<DateTime>()
            };
        }
    }
}