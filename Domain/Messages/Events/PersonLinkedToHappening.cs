namespace KsxEventTracker.Domain.Messages.Events
{
    using System;

    public class PersonLinkedToHappening : IEvent
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string RegistrationId { get; set; }
        public string PersonId { get; set; }
        public string HappeningId { get; set; }

        public PersonLinkedToHappening()
        {
            this.Timestamp = DateTime.UtcNow;
            this.Id = Guid.NewGuid().ToString();
        }

        public PersonLinkedToHappening(string personId, string happeningId, string registrationId)
            : this(Guid.NewGuid().ToString(), personId, happeningId, registrationId)
        {
        }

        public PersonLinkedToHappening(string id, string personId, string happeningId, string registrationId) :this()
        {
            this.Id = id;
            this.PersonId = personId;
            this.HappeningId = happeningId;
            this.RegistrationId = registrationId;
        }
    }
}