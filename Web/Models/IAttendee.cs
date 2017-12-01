namespace Web.Models
{
    using System;

    public interface IAttendee
    {
        Guid Id { get; set; }

        bool MayShowNameToPublic { get; set; }

        string Name { get; set; }
    }
}
