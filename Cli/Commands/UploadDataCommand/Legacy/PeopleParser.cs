namespace KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy
{
    using System;

    using KsxEventTracker.Cli.Commands.UploadDataCommand.Legacy.Models;

    public class PeopleParser : ParserBase<Person>
    {
        public PeopleParser(string csvRaw)
            : base(csvRaw, 10)
        {
        }

        public override Person ParseRow()
        {
            // Id;Name;Phone;Email;TwitterName;MayShowNameToPublic;HappeningsAttended;HappeningsCompleted;QuitLastTime
            return new Person
            {
                Id = this.Next<Guid>(),
                Name = this.Next(),
                Phone = this.Next(),
                Email = this.Next(),
                TwitterName = this.Next(),
                MayShowNameToPublic = this.Next<bool>(),
                HappeningsAttended = this.Next<int>(),
                HappeningsCompleted = this.Next<int>(),
                QuitLastTime = this.Next<bool>(),
                NewEventBasedId = this.Next()
            };
        }
    }
}