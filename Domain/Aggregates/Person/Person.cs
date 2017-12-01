namespace KsxEventTracker.Domain.Aggregates.Person
{
    using System;
    using System.Linq;

    using KsxEventTracker.Domain.Messages.Events;

    public class Person : AggregateRootBase<PersonState>
    {
        public Person(Action<IEvent> collectEvent, PersonState state)
            : base(collectEvent, state)
        {
        }

        // Creating persons individually, not validating uniqueness through a list or anything

        public void Create(
            string personId, 
            string nfcId,
            string lastname, 
            string firstname, 
            string displayName, 
            ContactInformation contactInformation,
            string info, 
            DateTimeOffset timestamp)
        {
            // Never fails, as everything has been checked already
            var e = new PersonCreated(
                personId, 
                nfcId,
                lastname, 
                firstname, 
                displayName, 
                contactInformation,
                info) { Timestamp = timestamp };

            this.Apply(e);
        }

        public void UpdateContactInformation(
            string personId, 
            string nfcId,
            string lastname, 
            string firstname, 
            string displayName, 
            ContactInformation contactInformation,
            string info, 
            DateTimeOffset timestamp)
        {
            var e = new PersonContactInformationUpdated(
                personId, 
                nfcId,
                lastname, 
                firstname, 
                displayName, 
                contactInformation,
                info) { Timestamp = timestamp };
            this.Apply(e);
        }

        public void LinkToHappening(string personId, string happeningId, string registrationId, DateTimeOffset timestamp)
        {
            if (this.State.Attendance.Contains(happeningId) || this.State.Deleted)
            {
                return;
            }

            var e = new PersonLinkedToHappening(personId, happeningId, registrationId)
                    {
                        Timestamp = timestamp
                    };
            this.Apply(e);
        }

        public void UnlinkFromHappening(string personId, string happeningId)
        {
            // Start of happening has been already validated, validate that this person is real and linked
            if (this.State.Deleted)
            {
                this.Apply(new PersonUnlinkFailed(happeningId, personId, "Henkilö on poistettu, linkitystä tapahtumaan ei voida poistaa."));
                return;
            }

            if (!this.State.Attendance.Contains(happeningId))
            {
                this.Apply(new PersonUnlinkFailed(happeningId, personId, "Henkilöä ei ole linkitetty tapahtumaan, linkitystä ei voida poistaa."));
                return;
            }

            this.Apply(new PersonUnlinkedFromHappening(personId, happeningId));
        }

        public void DeletePerson(string personId)
        {
            // Cannot use person's id as event id as that would conflict.
            this.Apply(new PersonDeleted(personId));
        }

        public static string CreateRandomEnoughBarcode()
        {
            return CreateRandomEnoughBarcode(new Random());
        }

        /// <summary>
        /// Use this overload if creating lots of barcodes in a loop (to avoid same barcodes).
        /// </summary>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static string CreateRandomEnoughBarcode(Random generator)
        {
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = generator;
            var result =
                new string(Enumerable.Repeat(Chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            return result;
        }
    }
}